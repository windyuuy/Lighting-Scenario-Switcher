using UnityEngine;

namespace MapLighting
{
	[System.Serializable]
	public class RendererRecorver
	{
		public int lightmapIndex;           // Parameter Verbose Name: LightMapIndex Color
		public Vector4 lightmapOffsetScale; // Parameter Verbose Name: LightMapOffsetScale
		
		public void Recover(Renderer renderer, BaseLightMapData baseLightMapData)
		{
			renderer.lightmapIndex = baseLightMapData.ValidateLightmapIndex(lightmapIndex);

			if (!renderer.isPartOfStaticBatch) {
				renderer.lightmapScaleOffset = lightmapOffsetScale;
			}
			if (renderer.isPartOfStaticBatch ) {
				Debug.LogError("Object " + renderer.gameObject.name + " is part of static batch, skipping lightmap offset and scale.");
			}
		}

		public void Collect(Renderer renderer, BaseLightMapData baseLightMapData)
		{
			if (BaseLightMapData.GetLightmapIndexValid(renderer.lightmapIndex)) {
				//info.renderer = renderer; // REMOVED - added uniqueId as the new lookup method
				this.lightmapOffsetScale = renderer.lightmapScaleOffset;

				var newLightmapsTextures = baseLightMapData.lightmaps;
				var newLightmapsTexturesDir = baseLightMapData.lightmapsDir;
				var newLightmapsTexturesShadow = baseLightMapData.lightmapsShadow;
				if (baseLightMapData.lightmapsMode != LightmapsMode.NonDirectional) {
					//first directional lighting
					Texture2D lightmapdir = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
					if (lightmapdir != null) {
						if (newLightmapsTexturesDir.IndexOf(lightmapdir) == -1) {
							newLightmapsTexturesDir.Add (lightmapdir);
						}
					}
					//now the shadowmask
					Texture2D lightmapshadow = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;
					if (lightmapshadow != null) {
						if (newLightmapsTexturesShadow.IndexOf(lightmapshadow) == -1) {
							newLightmapsTexturesShadow.Add (lightmapshadow);
						}
					}
				}
				Texture2D lightmaplight = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
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