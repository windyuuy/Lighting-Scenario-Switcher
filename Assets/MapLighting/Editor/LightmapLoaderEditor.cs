using UnityEditor;
using UnityEngine;

namespace MapLighting.Editor
{
	[CustomEditor(typeof(LightmapLoader))]
	public class LightmapLoaderEditor:UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			LightmapLoader loader = (LightmapLoader)target;
			
			if (GUILayout.Button("Load"))
			{
				loader.Load();
			}
		}
	}
}