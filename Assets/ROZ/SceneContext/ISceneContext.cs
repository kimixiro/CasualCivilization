using Cysharp.Threading.Tasks;

namespace ROZ.SceneContext
{
    public interface ISceneContext
    {
        UniTask Enter();
        UniTask Leave();
    }
}