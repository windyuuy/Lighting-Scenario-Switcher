using UnityEditor;
using UnityEngine;

namespace MapLighting
{
	public class LightmapSaver:MonoBehaviour
	{
		public string saveUrl;

		protected LightmapDataCollecter collecter = new();
		public async void Save()
		{
			collecter.Collect();
			await collecter.Save(saveUrl);
			AssetDatabase.Refresh();
			await collecter.DoPostSavedTask();
		}
	}
}