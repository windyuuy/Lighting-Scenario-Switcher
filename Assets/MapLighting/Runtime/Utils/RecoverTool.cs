using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace MapLighting
{
	public class RecoverTool
	{
		public static readonly RecoverTool Inst = new RecoverTool();

		public float[] SphericalHarmonicsL22ToArray(SphericalHarmonicsL2 sceneLightProbes)
		{
			var coefficients = new float[27];
			SphericalHarmonicsL22ToArray(sceneLightProbes, ref coefficients);
			return coefficients;
		}
		public void SphericalHarmonicsL22ToArray(SphericalHarmonicsL2 sceneLightProbes, ref float[] coefficients)
		{
			// j is coefficient
			for (int j = 0; j < 3; j++)	{
				//k is channel ( r g b )
				for (int k = 0; k < 9; k++)	{
					coefficients[j*9+k] = sceneLightProbes[j, k];
				}
			}
		}

		public SphericalHarmonicsL2 Array2SphericalHarmonicsL2(float[] coefficients)
		{
			SphericalHarmonicsL2 sphericalHarmonics=new SphericalHarmonicsL2();
			// j is coefficient
			for (int j = 0; j < 3; j++)	{
				//k is channel ( r g b )
				for (int k = 0; k < 9; k++)	{
					sphericalHarmonics[j, k] = coefficients[j * 9 + k];
				}
			}

			return sphericalHarmonics;
		}
		
		public static string GetObjPath(GameObject gameObject)
		{
			if (gameObject == null)
			{
				return null;
			}

			return GetObjPath(gameObject.transform);
		}
		public static string GetObjPath(Component component)
		{
			if (component == null)
			{
				return null;
			}

			return GetObjPath(component.transform);
		}
		public static string GetObjPath(Transform comp)
		{
			var strBuilder = new StringBuilder();
			var comp0 = comp;
			while(comp0.parent!=null)
			{
				strBuilder.Insert(0, comp0.gameObject.name);
				strBuilder.Insert(0, "/");
				comp0 = comp0.parent;
			}
			strBuilder.Insert(0, comp0.gameObject.name);

			return strBuilder.ToString();
		}

		public static T GetObjByPath<T>(string path) where T : Component
		{
			var obj = GetObjByPath(path);
			if (obj == null)
			{
				return null;
			}

			var comp = obj.GetComponent<T>();
			return comp;
		}
		public static GameObject GetObjByPath(string path)
		{
			var lines = path.Split("/");
			if (lines.Length <= 0)
			{
				return null;
			}

			for (var sceneIndex = SceneManager.sceneCount - 1; sceneIndex >= 0; sceneIndex--)
			{
				var scene = SceneManager.GetSceneAt(sceneIndex);
				var roots = scene.GetRootGameObjects();
				GameObject rootObj = roots.Reverse().FirstOrDefault(r => r.name == lines[0]);
				if (rootObj == null)
				{
					Debug.LogError($"lightmap target missing: {lines[0]}");
					continue;
				}

				Transform root = rootObj.transform;
				for (var i = 1; i < lines.Length; i++)
				{
					var name = lines[i];
					var found = false;
					for (var j = root.childCount - 1; j >= 0; j--)
					{
						var child = root.GetChild(j);
						if (child.gameObject.name == name)
						{
							found = true;
							root = child;
							break;
						}
					}

					if (!found)
					{
						continue;
					}
				}
				
				if (root == null)
				{
					continue;
				}

				return root.gameObject;
			}

			return null;
		}
		
		#if UNITY_EDITOR
		public static string CopySceneAsEmpty(Scene curScene,string destPath)
		{
			var newScenePath = destPath + curScene.name + "_Empty.unity";
			if (File.Exists(newScenePath))
			{
				// AssetDatabase.DeleteAsset(newScenePath);
				File.Copy(curScene.path, newScenePath,true);
			}
			else
			{
				AssetDatabase.CopyAsset(curScene.path, newScenePath);
			}
			var emptyScene = EditorSceneManager.OpenScene(newScenePath,OpenSceneMode.Additive);
			EditorSceneManager.SetActiveScene(emptyScene);
			foreach (var rootGameObject in emptyScene.GetRootGameObjects())
			{
				GameObject.DestroyImmediate(rootGameObject);
			}
			EditorSceneManager.SaveScene(emptyScene);
			EditorSceneManager.CloseScene(emptyScene,true);

			return newScenePath;
		}
		#endif
	}
}