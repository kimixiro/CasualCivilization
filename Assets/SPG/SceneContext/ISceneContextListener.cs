using Cysharp.Threading.Tasks;

namespace SPG.SceneContext
{
    public interface ISceneContextListener
    {
        public UniTask OnSceneEntered();

        public UniTask OnSceneLeaving();
    }
}