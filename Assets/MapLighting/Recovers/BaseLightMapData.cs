using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace MapLighting
{
	public class BaseLightMapData
	{
		public LightmapsMode lightmapsMode;

		public List<Texture2D> lightmaps=new();
		public List<Texture2D> lightmapsDir=new();
		public List<Texture2D> lightmapsShadow=new();

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
			var texture = lightmaps[lightmapIndex];
			var curLightmaps = LightmapSettings.lightmaps;
			for (var i = 0; i < curLightmaps.Length; i++)
			{
				if (curLightmaps[i].lightmapColor == texture)
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