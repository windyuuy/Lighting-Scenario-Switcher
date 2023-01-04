using UnityEngine;

namespace MapLighting
{
	[System.Serializable]
	public class RendererRecorver
	{
		public int lightmapIndex;           // Parameter Verbose Name: LightMapIndex Color
		public Vector4 lightmapOffsetScale; // Parameter Verbose Name: LightMapOffsetScale
		public int realtimeLightmapIndex;
		public Vector4 realtimeLightmapScaleOffset;
		
		public void Recover(MeshRenderer renderer, BaseLightMapData baseLightMapData)
		{
			if (BaseLightMapData.GetLightmapIndexValid(lightmapIndex))
			{
				renderer.lightmapIndex = baseLightMapData.ValidateLightmapIndex(lightmapIndex);
			}
			else
			{
				renderer.lightmapIndex = lightmapIndex;
			}
			if (!renderer.isPartOfStaticBatch) {
				renderer.lightmapScaleOffset = lightmapOffsetScale;
			}

			// if (BaseLightMapData.GetLightmapIndexValid(realtimeLightmapIndex))
			// {
			// 	renderer.realtimeLightmapIndex = baseLightMapData.ValidateLightmapIndex(realtimeLightmapIndex);
			// }
			// else
			// {
			// 	renderer.realtimeLightmapIndex = realtimeLightmapIndex;
			// }
			// if (!renderer.isPartOfStaticBatch) {
			// 	renderer.realtimeLightmapScaleOffset = realtimeLightmapScaleOffset;
			// }

			if (renderer.isPartOfStaticBatch ) {
				Logger.LogError(()=>"Object " + renderer.gameObject.name + " is part of static batch, skipping lightmap offset and scale.");
			}
		}

		public void Collect(Renderer renderer, BaseLightMapData baseLightMapData)
		{
			if (BaseLightMapData.GetLightmapIndexValid(renderer.lightmapIndex)) {
				var curLightmap = LightmapSettings.lightmaps[renderer.lightmapIndex];
				this.lightmapIndex=baseLightMapData.FindOrAddLightmap(curLightmap);
			}
			else
			{
				this.lightmapIndex = renderer.lightmapIndex;
			}
			this.lightmapOffsetScale = renderer.lightmapScaleOffset;

			if (BaseLightMapData.GetLightmapIndexValid(renderer.realtimeLightmapIndex))
			{
				var curRealtimeLightmap = LightmapSettings.lightmaps[renderer.realtimeLightmapIndex];
				this.realtimeLightmapIndex=baseLightMapData.FindOrAddLightmap(curRealtimeLightmap);
			}
			else
			{
				this.realtimeLightmapIndex = renderer.realtimeLightmapIndex;
			}
			this.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
		}
	}
}