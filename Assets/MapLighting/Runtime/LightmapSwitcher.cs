using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
    [DisallowMultipleComponent]
    public class LightmapSwitcher : MonoBehaviour
    {
        public string assetUrl;
        public bool saveTextureToLocal = false;

        public LightmapDataRecover recover;

        public static string fixAssetPath(string path)
        {
            if (!path.EndsWith("/"))
            {
                return path + "/";
            }
            return path;
        }
        public async void Load()
        {
            assetUrl = fixAssetPath(assetUrl);
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
            
            var newRecover = recover;
            if (newRecover == null)
            {
                var loadPath = assetUrl + "LightMapData.asset";
                newRecover = await Addressables.LoadAssetAsync<LightmapDataRecover>(loadPath).Task;
                if (newRecover != null)
                {
                    recover = newRecover;
                }
            }

            await newRecover.Load(assetUrl);
            newRecover.Recover(newRecover.BaseLightMapData);
        }

        public async void Save()
        {
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            assetUrl = fixAssetPath(assetUrl);
#if UNITY_EDITOR
            var collecter = await SaveLightmapData(assetUrl, saveTextureToLocal);
            recover = collecter;
#else
			throw new System.Exception("save valid only for editor");
#endif
        }

#if UNITY_EDITOR
        public static async Task<LightmapDataCollecter> SaveLightmapData(string saveUrl, bool saveTextureToLocal)
        {
            var collecter = ScriptableObject.CreateInstance<LightmapDataCollecter>();
            collecter.Collect();
            await collecter.Save(saveUrl, saveTextureToLocal);
            AssetDatabase.Refresh();
            await collecter.DoPostSavedTask();

            return collecter;
        }
#endif
    }
}