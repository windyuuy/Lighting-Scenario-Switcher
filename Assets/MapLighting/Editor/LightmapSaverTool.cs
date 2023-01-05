using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MapLighting.Editor
{
    public class LightmapSaverTool
    {
        public static Task<LightmapDataCollecter> SaveLightmapData(string saveUrl, bool saveTextureToLocal)
        {
            return LightmapSwitcher.SaveLightmapData(saveUrl, saveTextureToLocal);
        }
        public static bool IsPrefabInstance(UnityEngine.GameObject obj){
            var type = PrefabUtility.GetPrefabAssetType(obj);
            var status = PrefabUtility.GetPrefabInstanceStatus(obj);
            // 是否为预制体实例判断
            if (type == PrefabAssetType.NotAPrefab || status == PrefabInstanceStatus.NotAPrefab)
            {
                return false;
            }
            return true;
        }

        [MenuItem("Tools/Lightmap/Export")]
        public static async Task Save()
        {
            // if (Lightmapping.Bake())
            // {
            //     Debug.LogError("烘焙场景失败");
            //     return;
            // }
            
            var curScene=EditorSceneManager.GetActiveScene();
            var rootObjects=curScene.GetRootGameObjects();
            var prefabs = rootObjects.Where(obj =>
            {
                var isPrefab = IsPrefabInstance(obj);
                return isPrefab;
            }).ToArray();

            var scenePath = curScene.path;
            var sceneName=curScene.name;
            var defaultLightMapSavePath = Path.GetDirectoryName(scenePath).Replace("\\","/")+"/"+sceneName+"/";

            GameObject mapPrefab;
            if (prefabs.Length == 1)
            {
                mapPrefab=prefabs[0];
            }
            else if (prefabs.Length > 1)
            {
                mapPrefab = prefabs.FirstOrDefault(prefab =>
                {
                    return prefab.name == sceneName || prefab.name.Contains("Map");
                });
                if(mapPrefab==null)
                {
                    throw new Exception("无法检测确切有效的地图预制体节点");
                }
            }
            else
            {
                throw new Exception("未发现有效的地图预制体节点");
            }

            var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(mapPrefab);
            LightmapSwitcher switcher = mapPrefab.GetComponent<LightmapSwitcher>();
            var mapPrefab0 = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var switcher0 = mapPrefab0.GetComponent<LightmapSwitcher>();
            if (switcher == null)
            {
                if (switcher0 != null)
                {
                    PrefabUtility.RevertRemovedComponent(mapPrefab, switcher0, InteractionMode.UserAction);
                }
                
                switcher = mapPrefab.GetComponent<LightmapSwitcher>();
                if (switcher == null)
                {
                    switcher=mapPrefab.AddComponent<LightmapSwitcher>();
                    switcher.assetUrl = defaultLightMapSavePath;
                }
            }
            await switcher.SaveAsync();
            
            if (switcher0 == null)
            {
                PrefabUtility.ApplyAddedComponent(switcher, prefabPath, InteractionMode.UserAction);
            }
            else
            {
                switcher0.recover = switcher.recover;
                PrefabUtility.ApplyObjectOverride(switcher, prefabPath, InteractionMode.UserAction);
            }
            Debug.Log("export lightmap done");
        }
    }
}
