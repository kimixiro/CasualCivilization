using Cysharp.Threading.Tasks;

namespace ROZ.SceneContext.Options
{
    public interface IBuildableSceneContext
    {
        UniTask Build();
    }
}