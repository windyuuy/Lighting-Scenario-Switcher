using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MapLighting
{
	public class BaseLightMapData
	{
		public class LightmapInfo
		{
			public Texture2D lightmap;
			public Texture2D lightmapDir;
			public Texture2D lightmapShadow;
		}
		
		public LightmapsMode lightmapsMode;

		public List<LightmapInfo> lightmapInfos=new();

		public int FindOrAddLightmap(LightmapData curLightmap)
		{
			var newLightmapInfo = this.lightmapInfos.FirstOrDefault(info =>
			{
				return info.lightmap == curLightmap.lightmapColor
				       && info.lightmapDir == curLightmap.lightmapDir
				       && info.lightmapShadow == curLightmap.shadowMask;
			});
			if (newLightmapInfo == null)
			{
				newLightmapInfo = new LightmapInfo()
				{
					lightmap = curLightmap.lightmapColor,
					lightmapDir = curLightmap.lightmapDir,
					lightmapShadow = curLightmap.shadowMask,
				};
				this.lightmapInfos.Add(newLightmapInfo);
			}

			var index = this.lightmapInfos.IndexOf(newLightmapInfo);
			return index;
		}

		public static bool GetLightmapIndexValid (int lightmapIndex) {
			if (lightmapIndex != -1) {
				if (lightmapIndex != 0xFFFE) { // A value of 0xFFFE is internally used for objects that have their scale in lightmap set to 0.
					return true;
				}
			}
			return false;
		}

		public int ValidateLightmapIndex(int lightmapIndex)
		{
			var lightmapInfo = lightmapInfos[lightmapIndex];
			var curLightmaps = LightmapSettings.lightmaps;
			for (var i = 0; i < curLightmaps.Length; i++)
			{
				if (curLightmaps[i].lightmapColor == lightmapInfo.lightmap
				    &&curLightmaps[i].lightmapDir == lightmapInfo.lightmapDir
				    &&curLightmaps[i].shadowMask == lightmapInfo.lightmapShadow
				    )
				{
					return i;
				}
			}

			Debug.LogError($"missing lightmapIndex: {lightmapIndex}");
			return lightmapIndex;
		}

		public void Collect()
		{
			this.lightmapsMode = LightmapSettings.lightmapsMode;
		}
	}
}