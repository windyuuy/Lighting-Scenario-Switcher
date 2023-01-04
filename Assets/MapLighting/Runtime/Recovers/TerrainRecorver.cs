using System;
using UnityEngine;

namespace MapLighting
{
	[System.Serializable]
	public class TerrainRecorver
	{
		public int lightmapIndex;           // Parameter Verbose Name: LightMapIndex
		public Vector4 lightmapScaleOffset; // Parameter Verbose Name: LightmapScaleOffset
		public int realtimeLightmapIndex;
		public Vector4 realtimeLightmapScaleOffset;
		
		public void Recover(Terrain obj,BaseLightMapData baseLightMapData)
		{
			if (BaseLightMapData.GetLightmapIndexValid(obj.lightmapIndex))
			{
				obj.lightmapIndex = baseLightMapData.ValidateLightmapIndex(lightmapIndex);
			}
			else
			{
				obj.lightmapIndex = lightmapIndex;
			}
			obj.lightmapScaleOffset = lightmapScaleOffset;

			// if (BaseLightMapData.GetLightmapIndexValid(obj.realtimeLightmapIndex))
			// {
			// 	obj.realtimeLightmapIndex = baseLightMapData.ValidateLightmapIndex(realtimeLightmapIndex);
			// }
			// else
			// {
			// 	obj.realtimeLightmapIndex = realtimeLightmapIndex;
			// }
			// obj.realtimeLightmapScaleOffset = realtimeLightmapScaleOffset;
		}

		public void Collect(Terrain terrain,BaseLightMapData baseLightMapData)
		{
			if (BaseLightMapData.GetLightmapIndexValid(terrain.lightmapIndex)) {
				this.lightmapIndex = baseLightMapData.FindOrAddLightmap(LightmapSettings.lightmaps[terrain.lightmapIndex]);
			}
			else
			{
				this.lightmapIndex = terrain.lightmapIndex;
			}
			this.lightmapScaleOffset = terrain.lightmapScaleOffset;

			if (BaseLightMapData.GetLightmapIndexValid(terrain.realtimeLightmapIndex))
			{
				this.realtimeLightmapIndex = baseLightMapData.FindOrAddLightmap(LightmapSettings.lightmaps[terrain.realtimeLightmapIndex]);
			}
			else
			{
				this.realtimeLightmapIndex = terrain.realtimeLightmapIndex;
			}
			this.realtimeLightmapScaleOffset = terrain.realtimeLightmapScaleOffset;
		}
	}
}