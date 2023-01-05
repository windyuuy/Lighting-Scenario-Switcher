using System.Threading.Tasks;
using UnityEngine;

namespace MapLighting
{
	public class LightmapDataRecover:LightmapDataBase
	{
		public T GetCompByPath<T>(string path) where T : Component
		{
			var obj = RecoverTool.GetObjByPath(path);
			if (obj == null)
			{
				return null;
			}
			
			var comp = obj.GetComponent<T>();
			return comp;
		}

		public BaseLightMapData BaseLightMapData
		{
			get => baseLightMapData;
			set => baseLightMapData = value;
		}

		public async Task Load(string resourceUrl,bool loadScene)
		{
			if (loadScene)
			{
				await sceneData.Load();
			}
			baseLightMapData = new();
			// var loadPath = resourceUrl + "LightMapData.json";
			// var str = await Addressables.LoadAssetAsync<TextAsset>(loadPath).Task;
			// JsonUtility.FromJsonOverwrite(str.text, this);
			await lightmapData.Load(resourceUrl, baseLightMapData);
			await lightProbeData.Load(resourceUrl);
		}

		public void Recover(BaseLightMapData baseLightMapData)
		{
			lightmapData.Recover(baseLightMapData);
			
			renderSettingsesData.Recover();
			
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
				var comp = GetCompByPath<Renderer>(rendererData.path);
				if (comp != null)
				{
					rendererData.recover.Recover(comp,baseLightMapData);
				}
			}
			
			lightProbeData.Recover();

		}

		public async Task Unload()
		{
			sceneData.Unload();
		}
	}
}