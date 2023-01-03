using System;
using UnityEngine;

namespace MapLighting
{
	[System.Serializable]
	public class TerrainRecorver
	{
		public int lightmapIndex;           // Parameter Verbose Name: LightMapIndex
		public Vector4 lightmapScaleOffset; // Parameter Verbose Name: LightmapScaleOffset
		
		public void Recover(Terrain obj,BaseLightMapData baseLightMapData)
		{
			obj.lightmapIndex = baseLightMapData.ValidateLightmapIndex(lightmapIndex);
			obj.lightmapScaleOffset = lightmapScaleOffset;
		}

		public void Collect(Terrain terrain,BaseLightMapData baseLightMapData)
		{
			if (BaseLightMapData.GetLightmapIndexValid(terrain.lightmapIndex)) {
				this.lightmapScaleOffset = terrain.lightmapScaleOffset;

				var newLightmapsTextures = baseLightMapData.lightmaps;
				var newLightmapsTexturesDir = baseLightMapData.lightmapsDir;
				var newLightmapsTexturesShadow = baseLightMapData.lightmapsShadow;
				if (baseLightMapData.lightmapsMode != LightmapsMode.NonDirectional) {
					//first directional lighting
					Texture2D lightmapdir = LightmapSettings.lightmaps[terrain.lightmapIndex].lightmapDir;
					if (lightmapdir != null) {
						if (newLightmapsTexturesDir.IndexOf(lightmapdir) == -1) {
							newLightmapsTexturesDir.Add (lightmapdir);
						}
					}
					//now the shadowmask
					Texture2D lightmapshadow = LightmapSettings.lightmaps[terrain.lightmapIndex].shadowMask;
					if (lightmapshadow != null) {
						if (newLightmapsTexturesShadow.IndexOf(lightmapshadow) == -1) {
							newLightmapsTexturesShadow.Add (lightmapshadow);
						}
					}
				}
				Texture2D lightmaplight = LightmapSettings.lightmaps[terrain.lightmapIndex].lightmapColor;
				this.lightmapIndex = newLightmapsTextures.IndexOf(lightmaplight);
				if (lightmaplight != null) {
					if (newLightmapsTextures.IndexOf(lightmaplight) == -1) { // A value of -1 means no lightmap has been assigned, 
						this.lightmapIndex = newLightmapsTextures.Count;
						newLightmapsTextures.Add (lightmaplight);
					}
				}		
			}
		}
	}
}