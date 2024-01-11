using Cysharp.Threading.Tasks;

namespace SPG.SceneContext
{
    public interface ISceneContext
    {
        UniTask Enter();
        UniTask Leave();
    }
}