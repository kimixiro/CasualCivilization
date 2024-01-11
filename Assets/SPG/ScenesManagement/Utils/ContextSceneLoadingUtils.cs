using Cysharp.Threading.Tasks;
using SPG.Logging;
using SPG.SceneContext;
using SPG.SceneContext.Options;
using SPG.SceneContext.Utils;
using UnityEngine.SceneManagement;

namespace SPG.ScenesManagement.Utils
{
    public static class ContextSceneLoadingUtils
    {
        public static async UniTask<TSceneContext> LoadSceneSyncSingleAndBuild<TSceneContext>
        (
            int buildIndex, 
            bool isEnterContext = false
        )
            where TSceneContext : BaseSceneContext
        {
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncSingleAndBuild), 
                $"Start loading scene with index {buildIndex}");

            await SceneContextUtils.LeaveAllSceneContexts();

            await SceneLoadingUtils.LoadSceneSyncSingle(buildIndex);
            
            var loadedScene = SceneManager.GetActiveScene();
            
            var context = await SceneContextUtils.EnterSceneContext<TSceneContext>(loadedScene);
            
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncSingleAndBuild), 
                $"Scene loaded");
            
            if (isEnterContext)
            {
                Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncSingleAndBuild), 
                    $"Entering context");
                
                await context.Enter();
                
                Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncSingleAndBuild), 
                    $"Context entered");
            }
            return context;
        }

        public static async UniTask<TSceneContext> LoadSceneSyncAdditiveAndBuild<TSceneContext>
        (
            int buildIndex, 
            bool isMakeActive = false, 
            bool isEnterContext = false
        )
            where TSceneContext : BaseSceneContext, IBuildableSceneContext
        {
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncAdditiveAndBuild), 
                $"Start loading scene with index {buildIndex}");

            await SceneLoadingUtils.LoadSceneSyncAdditive(buildIndex, isMakeActive);
            
            var loadedScene = SceneManager.GetSceneByBuildIndex(buildIndex);
            
            var context = await SceneContextUtils.BuildSceneContext<TSceneContext>(loadedScene);
            
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncAdditiveAndBuild), 
                $"Scene loaded");
            
            if (isEnterContext)
            {
                Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncAdditiveAndBuild), 
                    $"Entering context");
                
                await context.Enter();
                
                Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneSyncAdditiveAndBuild), 
                    $"Context entered");
            }
            return context;
        }
        
        public static async UniTask<TSceneContext> LoadSceneAsyncAdditiveAndBuild<TSceneContext>
        (
            int buildIndex, 
            bool isMakeActive = false, 
            bool isEnterContext = false
        )
            where TSceneContext : BaseSceneContext, IBuildableSceneContext
        {
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneAsyncAdditiveAndBuild), 
                $"Start loading scene with index {buildIndex}");

            await SceneLoadingUtils.LoadSceneAsyncAdditive(buildIndex, isMakeActive);
            
            var loadedScene = SceneManager.GetSceneByBuildIndex(buildIndex);
            
            var context = await SceneContextUtils.BuildSceneContext<TSceneContext>(loadedScene);
            
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneAsyncAdditiveAndBuild), 
                $"Scene loaded");

            if (isEnterContext)
            {
                Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneAsyncAdditiveAndBuild), 
                    $"Entering context");
                
                await context.Enter();
                
                Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(LoadSceneAsyncAdditiveAndBuild), 
                    $"Context entered");
            }
            return context;
        }

        public static async UniTask UnloadSceneAsyncAndLeave(int buildIndex)
        {
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(UnloadSceneAsyncAndLeave), 
                $"Start unloading scene with index {buildIndex}");

            var sceneToLeave = SceneManager.GetSceneByBuildIndex(buildIndex);
            
            await SceneContextUtils.LeaveSceneContext<BaseSceneContext>(sceneToLeave);
            
            await SceneLoadingUtils.UnloadSceneAsync(buildIndex);
            
            Logger.LogInfo(nameof(ContextSceneLoadingUtils), nameof(UnloadSceneAsyncAndLeave), 
                $"Scene unloaded");
        }
    }
}