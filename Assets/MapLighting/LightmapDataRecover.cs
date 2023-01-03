using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace MapLighting
{
	public class LightmapDataRecover:LightmapDataBase
	{
		public T GetCompByPath<T>(string path) where T : Component
		{
			var obj = GetObjByPath(path);
			if (obj == null)
			{
				return null;
			}
			
			var comp = obj.GetComponent<T>();
			return comp;
		}
		public GameObject GetObjByPath(string path)
		{
			var lines = path.Split("/");
			if (lines.Length <= 0)
			{
				return null;
			}
			var roots=SceneManager.GetActiveScene().GetRootGameObjects();
			GameObject rootObj = roots.Reverse().FirstOrDefault(r => r.name == lines[0]);
			if (rootObj == null)
			{
				Debug.LogError($"lightmap target missing: {lines[0]}");
				return null;
			}
			
			Transform root = rootObj.transform;
			for (var i = 1; i < lines.Length; i++)
			{
				var name = lines[i];
				var found = false;
				for(var j=root.childCount-1;j>=0;j--)
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
					return null;
				}
			}

			if (root == null)
			{
				return null;
			}

			return root.gameObject;
		}

		public BaseLightMapData BaseLightMapData
		{
			get => baseLightMapData;
			set => baseLightMapData = value;
		}

		public async Task Load(string resourceUrl)
		{
			baseLightMapData = new();
			var loadPath = resourceUrl + "LightMapData.json";
			var str = await Addressables.LoadAssetAsync<TextAsset>(loadPath).Task;
			JsonUtility.FromJsonOverwrite(str.text, this);
			await lightmapData.Load(resourceUrl, baseLightMapData);
		}

		public void Recover(BaseLightMapData baseLightMapData)
		{
			lightProbeData.Recover();
			
			lightmapData.Recover(baseLightMapData);
			
			foreach (var lightData in lightDatas)
			{
				var comp = GetCompByPath<Light>(lightData.path);
				if (comp != null)
				{
					lightData.recover.Recover(comp);
				}
			}

			foreach (var lensFlareData in lensFlareDatas)
			{
				var comp = GetCompByPath<LensFlare>(lensFlareData.path);
				if (comp != null)
				{
					lensFlareData.recover.Recover(comp);
				}
			}
			
			foreach (var terrainData in terrainDatas)
			{
				var comp = GetCompByPath<Terrain>(terrainData.path);
				if (comp != null)
				{
					terrainData.recover.Recover(comp,baseLightMapData);
				}
			}
			
			foreach (var rendererData in rendererDatas)
			{
				var comp = GetCompByPath<MeshRenderer>(rendererData.path);
				if (comp != null)
				{
					rendererData.recover.Recover(comp,baseLightMapData);
				}
			}
		}
	}
}