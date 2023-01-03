using System;
using UnityEngine;

namespace MapLighting
{
	[Serializable]
	public class LightRecover
	{
		public bool enabled;               // Parameter Verbose Name: Enabled
		public float intensity;            // Parameter Verbose Name: Intensity
		public float range;                // Parameter Verbose Name: Range
		public float shadowStrength;       // Parameter Verbose Name: ShadowStrength
		public LightShadows shadows;       // Parameter Verbose Name: Shadows
		public Vector4 color;              // Parameter Verbose Name: Color
		
		public void Recover(Light component) { 
			component.enabled = this.enabled;                 	// Parameter Verbose Name: Enabled
			component.intensity = this.intensity;             	// Parameter Verbose Name: Intensity
			component.range = this.range;                     	// Parameter Verbose Name: Range
			component.shadows = this.shadows;                 	// Parameter Verbose Name: Shadows
			component.shadowStrength = this.shadowStrength;   	// Parameter Verbose Name: BakeType
			component.color = this.color;                     	// Parameter Verbose Name: Color
		}
		
		public  void Collect (Light component) {
			this.enabled = component.enabled;                  // Parameter Verbose Name: Enabled
			this.intensity = component.intensity;              // Parameter Verbose Name: Intensity
			this.range = component.range;                      // Parameter Verbose Name: Range
			this.shadows = component.shadows;                  // Parameter Verbose Name: Shadows
			this.shadowStrength = component.shadowStrength;    // Parameter Verbose Name: BakeType
			this.color = component.color;                      // Parameter Verbose Name: Color
		}

	}
}
