using UnityEngine;

namespace MapLighting
{
	public class LightmapLoader:MonoBehaviour
	{
		public string loadUrl;
		private LightmapDataRecover recover = new LightmapDataRecover();
		public async void Load()
		{
			await recover.Load(loadUrl);
			recover.Recover(recover.BaseLightMapData);
		}
	}
}