using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
	[System.Serializable]
	public class LightProbeRecover
	{
		// [System.Serializable]
		// public class LightProbeData
		// {
		// 	public float[] coefficients = new float[27];
		// }
		//
		// public LightProbeData[] data;
		
		public void Recover()
		{
			// var lightProbes = data;
			// var sphericalHarmonicsArray = new SphericalHarmonicsL2[lightProbes.Length];
			// for (int i = 0; i < lightProbes.Length; i++) {
			// 	var sphericalHarmonics = new SphericalHarmonicsL2();
			// 	// j is coefficient
			// 	for (int j = 0; j < 3; j++)	{
			// 		//k is channel ( r g b )
			// 		for (int k = 0; k < 9; k++)	{
			// 			sphericalHarmonics[j, k] = lightProbes[i].coefficients[j * 9 + k];
			// 		}
			// 	}
			// 	sphericalHarmonicsArray[i] = sphericalHarmonics; // Apply the changes
			// }
			//
			// try	{
			// 	LightmapSettings.lightProbes.bakedProbes = sphericalHarmonicsArray;
			// } catch { 
			// 	Debug.LogError("Error when trying to load lightprobes for scenario - Did you bake lighting before building the game?"); 
			// }
		}

		public void Collect()
		{
			// var newSphericalHarmonicsModelList = new List<LightProbeData>();
			// var sceneLightProbes = new SphericalHarmonicsL2[LightmapSettings.lightProbes.bakedProbes.Length];
			// sceneLightProbes = LightmapSettings.lightProbes.bakedProbes;
			//
			// for (int i = 0; i < sceneLightProbes.Length; i++) {
			// 	var SHCoeff = new LightProbeData();
			//
			// 	// j is coefficient
			// 	for (int j = 0; j < 3; j++)	{
			// 		//k is channel ( r g b )
			// 		for (int k = 0; k < 9; k++)	{
			// 			SHCoeff.coefficients[j*9+k] = sceneLightProbes[i][j, k];
			// 		}
			// 	}
			// 	newSphericalHarmonicsModelList.Add(SHCoeff);
			// }
			// this.data = newSphericalHarmonicsModelList.ToArray ();
		}

		public LightProbes lightProbes;
		public AssetReference lightProbesAsset;
		public async Task Load(string saveUrl)
		{
			if (lightProbes==null && saveUrl != null && lightProbesAsset!=null && lightProbesAsset.RuntimeKeyIsValid())
			{
				lightProbes=await lightProbesAsset.LoadAssetAsync<LightProbes>().Task;
			}
			LightmapSettings.lightProbes = lightProbes;
		}
		
		#if UNITY_EDITOR
		public void Save(string saveUrl)
		{
			var savePath = saveUrl + "LightProbe.asset";
			if (File.Exists(savePath))
			{
				AssetDatabase.DeleteAsset(savePath);
			}
			if (LightmapSettings.lightProbes != null)
			{
				lightProbes = GameObject.Instantiate(LightmapSettings.lightProbes);
				AssetDatabase.CreateAsset(lightProbes,savePath);
				var guid = AssetDatabase.AssetPathToGUID(savePath);
				lightProbesAsset = new AssetReference(guid);
			}
		}
		#endif

	}
}