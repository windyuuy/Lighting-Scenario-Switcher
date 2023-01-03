using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MapLighting
{
	public class RecoverTool
	{
		public static readonly RecoverTool Inst = new RecoverTool();

		public bool isVerbose = false;
		private string m_resourceStorageFolder = "LightMapData_1";
		private string m_resourceTemporaryFolder;

		//
		// File IO And Directory Lookups
		//
		private bool GetInputResoursesValid(object[] arr, int i)
		{
			if (arr[i] == null)
			{
				if (isVerbose == true)
				{
					Debug.Log("Null element in " + arr.GetType() + " at index " + i + " texture was null (Maybe it was the optional shadowmap?).");
				}

				return false;
			}

			return true;
		}

		private TextureImporter GetTextureImporter(Texture2D texture)
		{
			string newTexturePath = AssetDatabase.GetAssetPath(texture);
			TextureImporter importer = AssetImporter.GetAtPath(newTexturePath) as TextureImporter;
			return importer;
		}

		private Texture2D GetCurrentLightmapTexture(string name)
		{
			string path = AssetDatabase.GetAssetPath(LightmapSettings.lightmaps[0].lightmapColor); // Path to the currently loaded lightmap texture (the filename portion will be removed)
			path = path.Substring(0, path.Length - Path.GetFileName(path).Length); // Rremove the file name revealing only the path.
			path = Path.Combine(path, name); // Construct a path that assumes the destinatnion is the target texture of the current lightmap.
			return AssetDatabase.LoadAssetAtPath<Texture2D>(path); // Return the current light/direction/shadow map texture.
		}

		public string GetResourcesStorageDirectory()
		{
			return GetResourcesDirectory(m_resourceStorageFolder); // The directory where data resides.
		}

		public string GetResourcesTemporaryDirectory()
		{
			return GetResourcesDirectory(m_resourceTemporaryFolder); // The directory where data will be kept until all processing is finished.
		}

		private string GetResourcesDirectory(string _folder)
		{
			return _folder == null || _folder.Length == 0 ? "" : "Assets/Resources/" + _folder + "/"; // The directory where data will be kept until all processing is finished.
		}

		private void CopyTextureImporterProperties(Texture2D fromTexture, Texture2D toTexture)
		{
			TextureImporter fromTextureImporter = GetTextureImporter(fromTexture);
			TextureImporter toTextureImporter = GetTextureImporter(toTexture);

			toTextureImporter.wrapMode = fromTextureImporter.wrapMode;
			toTextureImporter.anisoLevel = fromTextureImporter.anisoLevel;
			toTextureImporter.sRGBTexture = fromTextureImporter.sRGBTexture;
			toTextureImporter.textureType = fromTextureImporter.textureType;
			toTextureImporter.textureCompression = fromTextureImporter.textureCompression;
		}

		public void CopyTextureToResources(string[] textures)
		{
			for (int i = 0; i < textures.Length; i++)
			{
				if (!GetInputResoursesValid(textures, i))
				{
					continue;
				}

				try
				{
					Texture2D texture = GetCurrentLightmapTexture(textures[i]); // Get texture by texture name.
					if (texture != null)
					{
						// Maybe the optional shadowmask didn't exist?
						string fromPath = AssetDatabase.GetAssetPath(texture); // Path to the currently loaded texture;

						string
							toPath = GetResourcesStorageDirectory() +
							         Path.GetFileName(fromPath); // First get a path without the index appended to the filename so we can check if from and to paths are the same. 

						if (fromPath.Equals(toPath))
						{
							//they are the exact same texture
							fromPath = toPath; // Make it so it will copy the texture from the old directory to the new temporary directory
						}

						string newName = texture.name + "_" + i; // Path was not the same as fromPath so now the index can be appended to the filename.
						toPath = GetResourcesTemporaryDirectory() + newName +
						         Path.GetExtension(AssetDatabase.GetAssetPath(texture)); // Now get the actual toPath in the resourses temp resources folder.

						AssetDatabase.CopyAsset(fromPath, toPath); // Create a copy of the currently loaded texture and place in the temp resources folder.

						AssetDatabase.Refresh(); // Refresh so the newTexture file can be found and loaded.
						Texture2D newTexture = Resources.Load<Texture2D>(m_resourceTemporaryFolder + "/" + newName); // Load the new texture as an object.

						CopyTextureImporterProperties(texture, newTexture); // Ensure new texture takes on same properties as origional texture.

						AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newTexture)); // Re-import texture file so it will be successfully compressed to desired format.
						EditorUtility.CompressTexture(newTexture, texture.format, TextureCompressionQuality.Best); // Now compress the texture.

						textures[i] = newTexture.name; // Set the new texture as the reference in the Json file.
						Resources.UnloadAsset(newTexture); // Since we will no longer be processing on this texture it can be unloaded.
					}
					else if (isVerbose)
					{
						Debug.Log("Texture " + textures[i] + " was not found in the lightmap textures.");
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Error while copying resources:" + textures.GetType().ToString() + "\nMaybe you forgot to bake lightmaps?");
				}
			}
		}
	}
}