using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
	[System.Serializable]
	public class LightMapRecover
	{
		[System.Serializable]
		public class LightmapInfo
		{
			public string lightmap;
			public string lightmapDir;
			public string lightmapShadow;
		}
		public LightmapsMode lightmapsMode;

		public LightmapInfo[] lightmapInfos;

		public async Task Load(string resourceFolder, BaseLightMapData baseLightMapData)
		{
			async Task<Texture2D> LoadTexture(string p)
			{
				if (string.IsNullOrEmpty(p))
				{
					return null;
				}
				var obj = await Addressables.LoadAssetAsync<Texture2D>(resourceFolder + p).Task;
				return obj;
			}

			async Task<Texture2D[]> LoadLightmap(LightmapInfo lightmapInfo)
			{
				var results=await Task.WhenAll(Enumerable.Empty<Task<Texture2D>>()
					.Append(LoadTexture(lightmapInfo.lightmap))
					.Append(LoadTexture(lightmapInfo.lightmapDir))
					.Append(LoadTexture(lightmapInfo.lightmapShadow))
				);
				return results;
			}

			// TODO: 优化同名文件重复加载
			var results =await Task.WhenAll(lightmapInfos.Select(LoadLightmap));
			baseLightMapData.lightmapsMode = this.lightmapsMode;
			for (var i = 0; i < lightmapInfos.Length; i++)
			{
				var result = results[i];
				baseLightMapData.lightmapInfos.Add(new BaseLightMapData.LightmapInfo()
				{
					lightmap = result[0],
					lightmapDir = result[1],
					lightmapShadow = result[2],
				});
			}
		}
		public void Recover(BaseLightMapData baseLightMapData)
		{
			var newLightmaps = new LightmapData[this.lightmapInfos.Length];
			for (int i = 0; i < newLightmaps.Length; i++)
			{
				var lightmapInfo = baseLightMapData.lightmapInfos[i];
				var newLightmap= new LightmapData();
				newLightmaps[i] = newLightmap;
				newLightmap.lightmapColor = lightmapInfo.lightmap;
				if (this.lightmapsMode != LightmapsMode.NonDirectional) {
					if (lightmapInfo.lightmapDir != null)
					{
						newLightmap.lightmapDir = lightmapInfo.lightmapDir;
					}

					if (lightmapInfo.lightmapShadow != null)
					{
						newLightmap.shadowMask = lightmapInfo.lightmapShadow;
					}
				}
			}
			LightmapSettings.lightmaps = newLightmaps;
			LightmapSettings.lightmapsMode = this.lightmapsMode;
		}

#if UNITY_EDITOR
		protected List<Func<Task>> PostTask;

		public async Task DoPostTask()
		{
			var tasks = PostTask.ToArray();
			PostTask.Clear();
			await Task.WhenAll(tasks.Select(call => call()));
		}

		public async Task Save(string resourceFolder, BaseLightMapData baseLightMapData, bool needSaveTexture)
		{
			if (PostTask == null)
			{
				PostTask = new();
			}

			async Task SaveTexture(Texture2D texture2D)
			{
				if (texture2D == null)
				{
					return;
				}
				
				var p = AssetDatabase.GetAssetPath(texture2D);
				var fileName = Path.GetFileName(p);
				if (!Directory.Exists(resourceFolder))
				{
					Directory.CreateDirectory(resourceFolder);
				}

				var destPath = $"{resourceFolder}{fileName}";
				// await using var destFile = File.Create(destPath);
				// await using var sourceFile = File.Open(p, FileMode.Open);
				// await sourceFile.CopyToAsync(destFile);
				AssetDatabase.CopyAsset(p, destPath);

				// coll[index] = destPath;

				async Task CopyProperies()
				{
					var toTextureImporter = AssetImporter.GetAtPath(destPath) as TextureImporter;
					var fromTextureImporter = AssetImporter.GetAtPath(p) as TextureImporter;

					toTextureImporter.wrapMode = fromTextureImporter.wrapMode;
					toTextureImporter.anisoLevel = fromTextureImporter.anisoLevel;
					toTextureImporter.sRGBTexture = fromTextureImporter.sRGBTexture;
					toTextureImporter.textureType = fromTextureImporter.textureType;
					toTextureImporter.textureCompression = fromTextureImporter.textureCompression;
				}

				PostTask.Add(CopyProperies);
			}

			async Task SaveLightmap(BaseLightMapData.LightmapInfo lightmapInfo)
			{
				await Task.WhenAll(Enumerable.Empty<Task>()
					.Append(SaveTexture(lightmapInfo.lightmap))
					.Append(SaveTexture(lightmapInfo.lightmapDir))
					.Append(SaveTexture(lightmapInfo.lightmapShadow))
				);
			}

			if (needSaveTexture)
			{
				// TODO: 优化同名文件冲突
				await Task.WhenAll(baseLightMapData.lightmapInfos.Select(SaveLightmap));
			}
		}
		
		public string GetTextureName (Texture2D texture2D) {
			if (texture2D != null)
			{
				var name=Path.GetFileName(AssetDatabase.GetAssetPath(texture2D));
				return name;
			}

			return null;
		}

		public void Collect(BaseLightMapData baseLightMapData)
		{
			var newLightmapsMode = baseLightMapData.lightmapsMode;
			this.lightmapsMode = newLightmapsMode;

			this.lightmapInfos = baseLightMapData.lightmapInfos.Select(lightmapInfo =>
			{
				return new LightmapInfo()
				{
					lightmap = GetTextureName(lightmapInfo.lightmap),
					lightmapDir = GetTextureName(lightmapInfo.lightmapDir),
					lightmapShadow = GetTextureName(lightmapInfo.lightmapShadow),
				};
			}).ToArray();
		}
		#endif
	}
}