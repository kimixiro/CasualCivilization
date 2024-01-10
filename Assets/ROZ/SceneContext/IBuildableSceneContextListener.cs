using Cysharp.Threading.Tasks;

namespace ROZ.SceneContext
{
    public interface IBuildableSceneContextListener
    {
        UniTask OnPostContextBuilt();
    }
}