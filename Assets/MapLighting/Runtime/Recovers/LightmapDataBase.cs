using System;
using UnityEngine;

namespace MapLighting
{
	[Serializable]
	public class LightmapDataBase:ScriptableObject
	{
		public Recoverable<LightRecover>[] lightDatas;
		public Recoverable<LensFlareRecover>[] lensFlareDatas;
		public Recoverable<TerrainRecorver>[] terrainDatas;
		public Recoverable<RendererRecorver>[] rendererDatas;
		public LightProbeRecover lightProbeData;
		public LightMapRecover lightmapData;
		public RenderSettingsRecover renderSettingsesData;
		public SceneRecover sceneData;
		[NonSerialized]
		public BaseLightMapData baseLightMapData;
	}
}	