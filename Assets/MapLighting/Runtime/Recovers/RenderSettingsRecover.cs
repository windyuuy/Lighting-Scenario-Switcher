using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace MapLighting
{
	[Serializable]
	public class RenderSettingsRecover
	{
		public bool fog;
		public float fogStartDistance;
		public float fogEndDistance;
		public FogMode fogMode;
		public Color fogColor;
		public float fogDensity;
		public AmbientMode ambientMode;
		public Color ambientSkyColor;
		public Color ambientEquatorColor;
		public Color ambientGroundColor;
		public float ambientIntensity;
		public Color ambientLight;
		public Color subtractiveShadowColor;
		public Material skybox;
		// public Light sun;
		public string sunRefPath;
		// public SphericalHarmonicsL2 ambientProbe;
		public float[] ambientProbeCo;
		public Texture customReflection;
		public float reflectionIntensity;
		public int reflectionBounces;
		// internal Cubemap defaultReflection;
		public DefaultReflectionMode defaultReflectionMode;
		public int defaultReflectionResolution;
		public float haloStrength;
		public float flareStrength;
		public float flareFadeSpeed;

		public void Recover()
		{
			RenderSettings.fog = this.fog;
			RenderSettings.fogStartDistance = this.fogStartDistance;
			RenderSettings.fogEndDistance = this.fogEndDistance;
			RenderSettings.fogMode = this.fogMode;
			RenderSettings.fogColor = this.fogColor;
			RenderSettings.fogDensity = this.fogDensity;
			RenderSettings.ambientMode = this.ambientMode;
			RenderSettings.ambientSkyColor = this.ambientSkyColor;
			RenderSettings.ambientEquatorColor = this.ambientEquatorColor;
			RenderSettings.ambientGroundColor = this.ambientGroundColor;
			RenderSettings.ambientIntensity = this.ambientIntensity;
			RenderSettings.ambientLight = this.ambientLight;
			RenderSettings.subtractiveShadowColor = this.subtractiveShadowColor;
			RenderSettings.skybox = this.skybox;
			if (this.sunRefPath != null)
			{
				var sun = RecoverTool.GetObjByPath<Light>(this.sunRefPath);
				RenderSettings.sun = sun;
			}
			
			RenderSettings.ambientProbe = RecoverTool.Inst.Array2SphericalHarmonicsL2(this.ambientProbeCo);
			RenderSettings.customReflection = this.customReflection;
			RenderSettings.reflectionIntensity = this.reflectionIntensity;
			RenderSettings.reflectionBounces = this.reflectionBounces;
			RenderSettings.defaultReflectionMode = this.defaultReflectionMode;
			RenderSettings.defaultReflectionResolution = this.defaultReflectionResolution;
			RenderSettings.haloStrength = this.haloStrength;
			RenderSettings.flareStrength = this.flareStrength;
			RenderSettings.flareFadeSpeed = this.flareFadeSpeed;
		}

		public void Collect()
		{
			this.fog = RenderSettings.fog;
			this.fogStartDistance = RenderSettings.fogStartDistance;
			this.fogEndDistance = RenderSettings.fogEndDistance;
			this.fogMode = RenderSettings.fogMode;
			this.fogColor = RenderSettings.fogColor;
			this.fogDensity = RenderSettings.fogDensity;
			this.ambientMode = RenderSettings.ambientMode;
			this.ambientSkyColor = RenderSettings.ambientSkyColor;
			this.ambientEquatorColor = RenderSettings.ambientEquatorColor;
			this.ambientGroundColor = RenderSettings.ambientGroundColor;
			this.ambientIntensity = RenderSettings.ambientIntensity;
			this.ambientLight = RenderSettings.ambientLight;
			this.subtractiveShadowColor = RenderSettings.subtractiveShadowColor;
			this.skybox = RenderSettings.skybox;
			// this.sun = RenderSettings.sun;
			this.sunRefPath = RecoverTool.GetObjPath(RenderSettings.sun);
			// this.ambientProbe = RenderSettings.ambientProbe;
			this.ambientProbeCo = RecoverTool.Inst.SphericalHarmonicsL22ToArray(RenderSettings.ambientProbe);
			this.customReflection = RenderSettings.customReflection;
			this.reflectionIntensity = RenderSettings.reflectionIntensity;
			this.reflectionBounces = RenderSettings.reflectionBounces;
			this.defaultReflectionMode = RenderSettings.defaultReflectionMode;
			this.defaultReflectionResolution = RenderSettings.defaultReflectionResolution;
			this.haloStrength = RenderSettings.haloStrength;
			this.flareStrength = RenderSettings.flareStrength;
			this.flareFadeSpeed = RenderSettings.flareFadeSpeed;
		}
	}
}