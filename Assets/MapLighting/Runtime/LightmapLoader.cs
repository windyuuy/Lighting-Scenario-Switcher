using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MapLighting
{
	public class LightmapLoader:MonoBehaviour
	{
		public string loadUrl;
		protected LightmapDataRecover recover;

		public async void Load()
		{
			try
			{
				if (Application.isPlaying)
				{
					await Addressables.InitializeAsync().Task;
				}
				else
				{
					Addressables.InitializeAsync().WaitForCompletion();
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
			var loadPath = loadUrl + "LightMapData.asset";
			recover = await Addressables.LoadAssetAsync<LightmapDataRecover>(loadPath).Task;
			await recover.Load(loadUrl);
			recover.Recover(recover.BaseLightMapData);
		}
	}
}