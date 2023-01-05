using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
    [DisallowMultipleComponent]
    public class LightmapSwitcher : MonoBehaviour
    {
        public string assetUrl;
        public bool loadRecoverScene = false;
#if UNITY_EDITOR
        public bool saveTextureToLocal = false;
        public string saveUrl;
#endif

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
            Debug.Log("Begin Load");
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
            
            Debug.Log("InitializeAsync done");
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

            Debug.Log("newRecover.Load begin");
            await newRecover.Load(assetUrl,loadRecoverScene);
            Debug.Log("newRecover.Recover begin");
            newRecover.Recover(newRecover.BaseLightMapData);
            Debug.Log("newRecover.Recover done");
        }

        public async Task Unload()
        {
            if (recover != null)
            {
                await recover.Unload();
            }
        }

        public async void Save()
        {
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
#if UNITY_EDITOR
            var savePath = saveUrl;
            if (string.IsNullOrWhiteSpace(savePath))
            {
                savePath = assetUrl;
            }
            savePath = fixAssetPath(savePath);
            var collecter = await SaveLightmapData(savePath, saveTextureToLocal);
            recover = collecter;
#else
			throw new System.Exception("save valid only for editor");
#endif
        }

#if UNITY_EDITOR
        public static async Task<LightmapDataCollecter> SaveLightmapData(string saveUrl, bool saveTextureToLocal)
        {
            var lightMapDataSavePath = LightmapDataCollecter.GetLightMapDataSavePath(saveUrl);
            LightmapDataCollecter collecter;
            if (File.Exists(lightMapDataSavePath))
            {
                collecter = AssetDatabase.LoadAssetAtPath<LightmapDataCollecter>(lightMapDataSavePath);
            }
            else
            {
                collecter = ScriptableObject.CreateInstance<LightmapDataCollecter>();
            }
            collecter.Collect();
            await collecter.Save(saveUrl, saveTextureToLocal);
            AssetDatabase.Refresh();
            await collecter.DoPostSavedTask();

            return collecter;
        }
#endif
    }
}