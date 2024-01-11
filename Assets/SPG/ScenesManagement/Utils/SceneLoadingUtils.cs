using System;
using Cysharp.Threading.Tasks;
using SPG.Logging;
using UnityEngine.SceneManagement;

namespace SPG.ScenesManagement.Utils
{
    public static class SceneLoadingUtils
    {
        public static async UniTask LoadSceneSyncSingle(int buildIndex)
        {
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneSyncSingle), 
                $"Start loading scene with index {buildIndex}");

            SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);            
            
            await UniTask.DelayFrame(1);
            
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneSyncSingle), 
                $"Scene loaded");
        }
        
        public static async UniTask LoadSceneAsyncSingle
        (
            int buildIndex, 
            Action<float> progress = null
        )
        {
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneAsyncSingle), 
                $"Start loading scene with index {buildIndex}");

            if (progress != null)
            {
                await SceneManager
                    .LoadSceneAsync(buildIndex, LoadSceneMode.Single)
                    .ToUniTask(new Progress<float>(progress));            
            }
            else
            {
                await SceneManager
                    .LoadSceneAsync(buildIndex, LoadSceneMode.Single)
                    .ToUniTask();            
            }
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneAsyncSingle), 
                $"Scene loaded");
        }
        
        public static async UniTask LoadSceneSyncAdditive
        (
            int buildIndex, 
            bool isMakeActive = false
        )
        {
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneSyncAdditive), 
                $"Start loading scene with index {buildIndex}");

            SceneManager.LoadScene(buildIndex, LoadSceneMode.Additive);            
                
            await UniTask.DelayFrame(1);
            
            if (isMakeActive)
            {
                var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
                SceneManager.SetActiveScene(scene);
            }
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneSyncAdditive), 
                $"Scene loaded");
        }
        
        public static async UniTask LoadSceneAsyncAdditive
        (
            int buildIndex, 
            bool isMakeActive = false, 
            Action<float> progress = null
        )
        {
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneAsyncAdditive), 
                $"Start loading scene with index {buildIndex}");

            if (progress != null)
            {
                await SceneManager
                    .LoadSceneAsync(buildIndex, LoadSceneMode.Additive)
                    .ToUniTask(new Progress<float>(progress));            
            }
            else
            {
                await SceneManager
                    .LoadSceneAsync(buildIndex, LoadSceneMode.Additive)
                    .ToUniTask();            
            }
            if (isMakeActive)
            {
                var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
                SceneManager.SetActiveScene(scene);
            }
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(LoadSceneAsyncAdditive), 
                $"Scene loaded");
        }

        public static async UniTask UnloadSceneAsync(int buildIndex, Action<float> progress = null)
        {
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(UnloadSceneAsync), 
                $"Start unloading scene with index {buildIndex}");

            if (progress != null)
            {
                await SceneManager
                    .UnloadSceneAsync(buildIndex)
                    .ToUniTask(new Progress<float>(progress));            
            }
            else
            {
                await SceneManager
                    .UnloadSceneAsync(buildIndex)
                    .ToUniTask();            
            }
            Logger.LogInfo(nameof(SceneLoadingUtils), nameof(UnloadSceneAsync), 
                $"Scene unloaded");
        }
    }
}