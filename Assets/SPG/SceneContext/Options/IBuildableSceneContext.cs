using Cysharp.Threading.Tasks;

namespace SPG.SceneContext.Options
{
    public interface IBuildableSceneContext
    {
        UniTask Build();
    }
}