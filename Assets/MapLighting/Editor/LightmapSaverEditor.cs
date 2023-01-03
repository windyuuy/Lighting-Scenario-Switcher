using UnityEditor;
using UnityEngine;

namespace MapLighting.Editor
{
	[CustomEditor(typeof(LightmapSaver))]
	public class LightmapSaverEditor:UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			LightmapSaver saver = (LightmapSaver)target;
			
			if (GUILayout.Button("Save"))
			{
				saver.Save();
			}
		}
	}
}