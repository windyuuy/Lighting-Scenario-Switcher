using UnityEngine;

namespace MapLighting
{
	[System.Serializable]
	public class LensFlareRecover
	{
		public bool enabled;               // Parameter Verbose Name: Enabled
		public Vector4 color;              // Parameter Verbose Name: Color
		public float brightness;		   // Parameter Verbose Name: Brightness
		public float fadeSpeed;            // Parameter Verbose Name: FadeSpeed
		
		public void Recover(LensFlare component) {
			component.enabled = this.enabled;
			component.color = this.color;
			component.brightness = this.brightness;
			component.fadeSpeed = this.fadeSpeed;
		}

		public void Collect(LensFlare component)
		{
			this.enabled = component.enabled;
			this.color = component.color;
			this.brightness = component.brightness;
			this.fadeSpeed = component.fadeSpeed;
		}

	}
}