using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using SPG.SceneContext;
using SPG.UI.Presenters;
using UnityEngine.Assertions;

namespace SPG.UI
{
    public abstract class BaseSceneUIPresenter 
        : ISceneContextListener
        , IBuildableSceneContextListener
    {
        protected UIPresenter UiPresenter { get; private set; }

        public BaseSceneUIPresenter([NotNull] UIPresenter uiPresenter)
        {
            Assert.IsNotNull(uiPresenter);
            
            UiPresenter = uiPresenter;
        }

        public virtual UniTask OnSceneEntered()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnSceneLeaving()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask OnPostContextBuilt()
        {
            return UniTask.CompletedTask;
        }
    }
}