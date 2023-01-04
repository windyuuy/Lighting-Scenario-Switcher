using System;
using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
	public class LightmapSaver:MonoBehaviour
	{
		public string saveUrl;
		public bool saveTextureToLocal=false;

		protected LightmapDataCollecter collecter;
		[Conditional("UNITY_EDITOR")]
		public async void Save()
		{
			#if UNITY_EDITOR
			collecter = ScriptableObject.CreateInstance<LightmapDataCollecter>();
			collecter.Collect();
			await collecter.Save(saveUrl, saveTextureToLocal);
			AssetDatabase.Refresh();
			await collecter.DoPostSavedTask();
#else
			throw new System.Exception("save valid only for editor");
#endif
		}
	}
}