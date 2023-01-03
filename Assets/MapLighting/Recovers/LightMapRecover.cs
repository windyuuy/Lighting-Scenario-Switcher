using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MapLighting
{
	[System.Serializable]
	public class LightMapRecover
	{
		public LightmapsMode lightmapsMode;

		public string[] lightmaps;
		public string[] lightmapsDir;
		public string[] lightmapsShadow;

		public async Task Load(string resourceFolder, BaseLightMapData baseLightMapData)
		{
			async Task<Texture2D> LoadTexture(string p)
			{
				var obj = await Addressables.LoadAssetAsync<Texture2D>(resourceFolder + p).Task;
				return obj;
			}
			var task1=Task.WhenAll(lightmaps.Select(LoadTexture));
			var task2=Task.WhenAll(lightmapsDir.Select(LoadTexture));
			var task3=Task.WhenAll(lightmapsShadow.Select(LoadTexture));
			var result=await Task.WhenAll(Enumerable.Empty<Task<Texture2D[]>>().Append(task1).Append(task2).Append(task3));
			baseLightMapData.lightmapsMode = this.lightmapsMode;
			baseLightMapData.lightmaps = new List<Texture2D>(result[0]);
			baseLightMapData.lightmapsDir = new List<Texture2D>(result[1]);
			baseLightMapData.lightmapsShadow = new List<Texture2D>(result[2]);
		}

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

			Func<Texture2D, int, Task> PreSaveTexture(IList<string> coll)
			{
				async Task SaveTexture(Texture2D texture2D, int index)
				{
					var p = AssetDatabase.GetAssetPath(texture2D);
					var fileName = Path.GetFileName(p);
					if (!Directory.Exists(resourceFolder))
					{
						Directory.CreateDirectory(resourceFolder);
					}

					var destPath = $"{resourceFolder}{fileName}";
					await using var destFile = File.Create(destPath);
					await using var sourceFile = File.Open(p, FileMode.Open);
					await sourceFile.CopyToAsync(destFile);

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

				return SaveTexture;
			}

			if (needSaveTexture)
			{
				var task1=Task.WhenAll(baseLightMapData.lightmaps.Select(PreSaveTexture(lightmaps)));
				var task2=Task.WhenAll(baseLightMapData.lightmapsDir.Select(PreSaveTexture(lightmapsDir)));
				var task3=Task.WhenAll(baseLightMapData.lightmapsShadow.Select(PreSaveTexture(lightmapsShadow)));
				await Task.WhenAll(Enumerable.Empty<Task<Texture2D[]>>().Append(task1).Append(task2).Append(task3));
			}
		}
		public void Recover(BaseLightMapData baseLightMapData)
		{
			var newLightmaps = new LightmapData[this.lightmaps.Length];
			for (int i = 0; i < newLightmaps.Length; i++) {
				newLightmaps[i] = new LightmapData();
				newLightmaps[i].lightmapColor = baseLightMapData.lightmaps[i];
				if (this.lightmapsMode != LightmapsMode.NonDirectional) {
					if (this.lightmapsDir.Length > i && this.lightmapsDir [i] != null) { // If the textuer existed and was set in the data file.
						newLightmaps[i].lightmapDir = baseLightMapData.lightmapsDir[i];
					}
					if (this.lightmapsShadow.Length > i && this.lightmapsShadow [i] != null) { // If the textuer existed and was set in the data file.
						newLightmaps[i].shadowMask = baseLightMapData.lightmapsShadow[i];
					}
				}
			}
			LightmapSettings.lightmaps = newLightmaps;
			LightmapSettings.lightmapsMode = this.lightmapsMode;
		}

		public string[] TextureListToArrayOfNames (List<Texture2D> inArray) {
			string[] outArray = new string[inArray.Count];
			for(int i = 0; i < inArray.Count; i++) {
				if (inArray [i] != null) {
					outArray [i] = Path.GetFileName(AssetDatabase.GetAssetPath(inArray [i]));
				}
			}
			return outArray;
		}

		public void Collect(BaseLightMapData baseLightMapData)
		{
			var newLightmapsMode = baseLightMapData.lightmapsMode;
			var newLightmapsTextures = baseLightMapData.lightmaps;
			var newLightmapsTexturesDir = baseLightMapData.lightmapsDir;
			var newLightmapsTexturesShadow = baseLightMapData.lightmapsShadow;
			this.lightmapsMode = newLightmapsMode;
			this.lightmaps = TextureListToArrayOfNames(newLightmapsTextures);

			if (newLightmapsMode != LightmapsMode.NonDirectional) {
				this.lightmapsDir = TextureListToArrayOfNames(newLightmapsTexturesDir);
				this.lightmapsShadow = TextureListToArrayOfNames(newLightmapsTexturesShadow);
			} else {
				// if (isVerbose == true) {
					Debug.LogWarning("Lightmap settings are non-directional. Not saving directional and shadow textures.");
				// }
			}
		}
	}
}