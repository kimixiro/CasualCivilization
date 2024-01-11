using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using Logger = SPG.Logging.Logger;

namespace SPG.SceneContext
{
    public abstract class BaseSceneContext
        : MonoBehaviour
        , ISceneContext
    {
        private IReadOnlyList<ISceneContextListener> _listeners;

        [SerializeField] private bool _isAutoStart;

        private void Start()
        {
            if (_isAutoStart)
            {
                AutoStart().Forget();
            }
        }

        private void OnValidate()
        {
            name = $"[{GetType().Name}-SceneContext]";
        }

        private async UniTask AutoStart()
        {
            Logger.LogInfo(nameof(BaseSceneContext), nameof(AutoStart), 
                "Auto-starting starts", this);
            
            await Enter();
        
            Logger.LogInfo(nameof(BaseSceneContext), nameof(AutoStart), 
                "Auto-starting complete", this);
        }

        [Inject]
        public void Construct([NotNull] IReadOnlyList<ISceneContextListener> listeners)
        {
            Assert.IsNotNull(listeners);
            
            _listeners = listeners;
        }

        public virtual async UniTask Enter()
        {
            foreach (var context in _listeners)
            {
                await context.OnSceneEntered();
            }
        }

        public virtual async UniTask Leave()
        {
            foreach (var context in _listeners)
            {
                await context.OnSceneLeaving();
            }
        }
    }
}