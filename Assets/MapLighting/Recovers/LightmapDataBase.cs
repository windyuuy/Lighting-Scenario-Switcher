using System;

namespace MapLighting
{
	[Serializable]
	public class LightmapDataBase
	{
		public Recoverable<LightRecover>[] lightDatas;
		public Recoverable<LensFlareRecover>[] lensFlareDatas;
		public Recoverable<TerrainRecorver>[] terrainDatas;
		public Recoverable<RendererRecorver>[] rendererDatas;
		public LightProbeRecover lightProbeData;
		public LightMapRecover lightmapData;
		[NonSerialized]
		public BaseLightMapData baseLightMapData;
	}
}	