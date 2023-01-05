using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IngameDebugConsole.Commands
{
	public class GameObjectCommands
	{
		public static GameObject[] GetRootGameObjects()
		{
			List<GameObject> roots = new();
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				var root1=scene.GetRootGameObjects();
				roots.AddRange(root1);
			}

			return roots.ToArray();
		}
		public static GameObject FindGameObject(string path)
		{
			var ps = path.Split("/");
			var rootName = ps[0];
			GameObject root = null;
			for (var i = 0; i < SceneManager.sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);
				var roots = scene.GetRootGameObjects();
				root = roots.FirstOrDefault(r => r.name == rootName);
				if (root != null)
				{
					break;
				}
			}

			var curNode = root;
			for(var i=1;i<ps.Length;i++)
			{
				var p = ps[i];
				for (var j = 0; j < curNode.transform.childCount; j++)
				{
					var child = curNode.transform.GetChild(j).gameObject;
					if (child.name == p)
					{
						curNode = child;
						break;
					}
				}
			}

			return curNode;
		}
		[ConsoleMethod( "obj.show", "" ), UnityEngine.Scripting.Preserve]
		public static string Show( bool active, string key )
		{
			var obj = FindGameObject(key);
			if (obj != null)
			{
				obj.SetActive(active);
			}
			return "";
		}

		[ConsoleMethod("obj.ls", ""), UnityEngine.Scripting.Preserve]
		public static string ListGameObjects()
		{
			return ListGameObjects("");
		}
		[ConsoleMethod( "obj.ls", "" ), UnityEngine.Scripting.Preserve]
		public static string ListGameObjects(string key)
		{
			List<string> cs = null;
			if (string.IsNullOrWhiteSpace(key))
			{
				cs = GetRootGameObjects().Select(obj => obj.name).ToList();
			}
			else
			{
				var parent = FindGameObject(key);
				if (parent != null)
				{
					cs = new List<string>();
					for (var i = 0; i < parent.transform.childCount; i++)
					{
						var child = parent.transform.GetChild(i);
						cs.Add(child.name);
					}
				}
			}

			if (cs != null)
			{
				return $"{{\n\t{string.Join("\n\t", cs)}\n}}";
			}

			return "{0}";
		}

		[ConsoleMethod("obj.enable", ""), UnityEngine.Scripting.Preserve]
		public static string EnableComponent(string key, string compStr, bool enable)
		{
			var parent = FindGameObject(key);
			if (parent != null)
			{
				var comp=parent.GetComponent(compStr);
				if (comp is MonoBehaviour mcomp)
				{
					mcomp.enabled = enable;
				}
			}

			return "";
		}
	}
}