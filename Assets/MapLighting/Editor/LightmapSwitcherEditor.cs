using UnityEditor;
using UnityEngine;

namespace MapLighting.Editor
{
	[CustomEditor(typeof(LightmapSwitcher))]
	public class LightmapSwitcherEditor:UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			LightmapSwitcher saver = (LightmapSwitcher)target;
			
			GUILayout.Space(10);
			if (GUILayout.Button("Load"))
			{
				saver.Load();
			}

			GUILayout.Space(10);
			
			if (GUILayout.Button("Save"))
			{
				saver.Save();
			}
		}
	}
}