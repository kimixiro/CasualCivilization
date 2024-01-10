using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using ROZ.SceneContext.Options;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;
using VContainer.Unity;

namespace ROZ.SceneContext
{
    public abstract class BuildableSceneContext<TScope>
        : BaseSceneContext
        , IBuildableSceneContext
        where TScope : LifetimeScope
    {
        private IReadOnlyList<IBuildableSceneContextListener> _buildableListeners;
        
        [SerializeField] private bool _isAutoBuild;
        [SerializeField] private TScope _scope;

        private void Awake()
        {
            if (_isAutoBuild)
            {
                Build().Forget();
            }
        }

        protected virtual UniTask OnPreBuild()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnPostBuild()
        {
            return UniTask.CompletedTask;
        }

        [Inject]
        public void Construct([NotNull] IReadOnlyList<IBuildableSceneContextListener> buildableListeners)
        {
            Assert.IsNotNull(buildableListeners);
            
            _buildableListeners = buildableListeners;
        }

        public async UniTask Build()
        {
            await OnPreBuild();

            _scope.Build();
            
            await OnPostBuild();
            
            foreach (var listener in _buildableListeners)
            {
                await listener.OnPostContextBuilt();
            }
        }
    }
}