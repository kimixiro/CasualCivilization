using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using SPG.SceneContext.Options;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = SPG.Logging.Logger;
using Object = UnityEngine.Object;

namespace SPG.SceneContext.Utils
{
    public static class SceneContextUtils
    {
        [NotNull]
        private static TSceneContext GetSceneContext<TSceneContext>(Scene scene)
            where TSceneContext : BaseSceneContext
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                throw new InvalidOperationException($"Used invalid scene {scene.name}");
            }
            foreach (var gameObject in scene.GetRootGameObjects())
            {
                var context = gameObject.GetComponentInChildren<TSceneContext>();

                if (context != null)
                {
                    return context;
                }
            }
            throw new InvalidOperationException(
                $"Cant find entry point {typeof(TSceneContext).Name} in scene {scene.name}");
        }

        public static async UniTask<TSceneContext> BuildSceneContext<TSceneContext>(Scene scene)
            where TSceneContext : BaseSceneContext, IBuildableSceneContext
        {
            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Start entering point {typeof(TSceneContext).Name}");
            
            var context = GetSceneContext<TSceneContext>(scene);

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Entering {context.name}");

            await context.Build();

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Scope entered");

            return context;
        }
        
        public static async UniTask<TSceneContext> EnterSceneContext<TSceneContext>(Scene scene)
            where TSceneContext : BaseSceneContext
        {
            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Start entering point {typeof(TSceneContext).Name}");
            
            var context = GetSceneContext<TSceneContext>(scene);

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Entering {context.name}");

            await context.Enter();

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Scope entered");

            return context;
        }
        
        public static async UniTask<TSceneContext> EnterSceneContext<TSceneContext, TData>(Scene scene, TData data)
            where TSceneContext : BaseSceneContext, IHasData<TData>
            where TData : struct
        {
            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Start entering point {typeof(TSceneContext).Name}");

            var context =  GetSceneContext<TSceneContext>(scene);

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Entering {context.name}");

            context.SetData(data);
            
            await context.Enter();

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Scope entered");

            return context;
        }
        
        public static async UniTask<TSceneContext> LeaveSceneContext<TSceneContext>(Scene scene)
            where TSceneContext : BaseSceneContext
        {
            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Start leaving entry point {typeof(TSceneContext).Name}");

            var context = GetSceneContext<TSceneContext>(scene);

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"leaving {context.name}");

            await context.Leave();

            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Entry point leaved");
            
            return context;
        }
        
        public static async UniTask LeaveAllSceneContexts()
        {
            Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                $"Start leaving all entry points");

            var contexts = Object.FindObjectsByType<BaseSceneContext>(
                FindObjectsSortMode.None);

            foreach (var point in contexts)
            {
                Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                    $"Leaving {point.name}");

                await point.Leave();
                
                Logger.LogInfo(nameof(SceneContextUtils), nameof(EnterSceneContext), 
                    $"Entry point leaved");
            }
        }
    }
}