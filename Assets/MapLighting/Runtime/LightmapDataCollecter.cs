using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
	[System.Serializable]
	public class LightmapDataCollecter : LightmapDataRecover
	{
		#if UNITY_EDITOR
		public async Task DoPostSavedTask()
		{
			await lightmapData.DoPostTask();
		}
		public void Collect()
		{
			baseLightMapData = new();
			baseLightMapData.Collect();

			renderSettingsesData = new();
			renderSettingsesData.Collect();
			
			// 收集光照设置
			var lightParams = GameObject.FindObjectsOfType<Light>();
			var lightDatas0 = lightParams.Select(light =>
			{
				var lightData = new LightRecover();
				lightData.Collect(light);
				return Recoverable.Conv(light, lightData);
			}).ToArray();

			// 收集光晕设置
			var flareParams = GameObject.FindObjectsOfType<LensFlare>();
			var lensFlareDatas0 = flareParams.Select(flare =>
			{
				var lensFlareData = new LensFlareRecover();
				lensFlareData.Collect(flare);
				return Recoverable.Conv(flare, lensFlareData);
			}).ToArray();

			// 收集terrain信息
			var terrainParams = GameObject.FindObjectsOfType<Terrain>();
			var terrainDatas0 = terrainParams.Select(terrain =>
			{
				var terrainData = new TerrainRecorver();
				terrainData.Collect(terrain, baseLightMapData);
				return Recoverable.Conv(terrain, terrainData);
			}).ToArray();

			// 收集render信息
			var rendererParams = GameObject.FindObjectsOfType<Renderer>(true);
			var rendererDatas0 = rendererParams.Select(renderer =>
			{
				var rendererData = new RendererRecorver();
				rendererData.Collect(renderer, baseLightMapData);
				return Recoverable.Conv(renderer, rendererData);
			}).ToArray();

			// 收集光照探针信息
			var lightProbeData0 = new LightProbeRecover();
			lightProbeData0.Collect();

			// 收集光照贴图信息
			var lightmapData0 = new LightMapRecover();
			lightmapData0.Collect(baseLightMapData);

			var sceneData0 = new SceneRecover();

			this.lightDatas = lightDatas0;
			this.lensFlareDatas = lensFlareDatas0;
			this.terrainDatas = terrainDatas0;
			this.rendererDatas = rendererDatas0;
			this.lightProbeData = lightProbeData0;
			this.lightmapData = lightmapData0;
			this.sceneData = sceneData0;
		}
		#endif

		public static string GetLightMapDataSavePath(string saveDir)
		{
			var lightMapDataSavePath = saveDir + "LightMapData.asset";
			return lightMapDataSavePath;
		}
		public async Task Save(string saveDir,bool needSaveTexture)
		{
#if UNITY_EDITOR
			if (!Directory.Exists(saveDir))
			{
				Directory.CreateDirectory(saveDir);
			}
			
			var task1=lightmapData.Save(saveDir, baseLightMapData, needSaveTexture);
			await Task.WhenAll(Enumerable.Empty<Task>()
				.Append(task1)
				// .Append(task2)
			);
			lightProbeData.Save(saveDir);
			
			sceneData.Save(saveDir);
			
			var lightMapDataSavePath = GetLightMapDataSavePath(saveDir);
			var selfAssetPath = AssetDatabase.GetAssetPath(this);
			if (lightMapDataSavePath==selfAssetPath && string.IsNullOrEmpty(selfAssetPath)==false && File.Exists(selfAssetPath))
			{
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssetIfDirty(this);
			}
			else
			{
				AssetDatabase.CreateAsset(this, lightMapDataSavePath);
			}

#else
			throw new System.Exception("save valid only for editor");
#endif
		}
	}
}