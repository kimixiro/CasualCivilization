using Cysharp.Threading.Tasks;

namespace ROZ.SceneContext
{
    public interface ISceneContextListener
    {
        public UniTask OnSceneEntered();

        public UniTask OnSceneLeaving();
    }
}