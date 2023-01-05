using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MapLighting
{
    [Serializable]
    public class SceneRecover
    {
        public AssetReference sceneAsset;
        [NonSerialized]
        protected UnityEngine.ResourceManagement.ResourceProviders.SceneInstance sceneInstance;

        public async Task Load()
        {
            if (sceneAsset != null)
            {
                sceneInstance=await sceneAsset.LoadSceneAsync(LoadSceneMode.Additive).Task;
                SceneManager.SetActiveScene(sceneInstance.Scene);
            }
        }
        
        public async Task Unload()
        {
            await sceneAsset.UnLoadScene().Task;
            sceneAsset.ReleaseAsset();
        }

        public void Save(string saveUrl)
        {
#if UNITY_EDITOR
            var curScene = SceneManager.GetActiveScene();
            var scenePath = curScene.path;
            var sceneName=curScene.name;
            var defaultLightMapSavePath = saveUrl;
            var newScenePath=RecoverTool.CopySceneAsEmpty(curScene, defaultLightMapSavePath);
            var guid = AssetDatabase.AssetPathToGUID(newScenePath);
            sceneAsset = new AssetReference(guid);
#endif
        }
    }
}