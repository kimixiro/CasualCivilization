using Cysharp.Threading.Tasks;

namespace SPG.SceneContext
{
    public interface IBuildableSceneContextListener
    {
        UniTask OnPostContextBuilt();
    }
}