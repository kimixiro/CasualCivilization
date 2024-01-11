using System;
using Cysharp.Threading.Tasks;

namespace SPG.UI.Presenters
{
    public interface IShowable
    {
        event Action<IShowable> Hidden; 
        event Action<IShowable> Showed;
        
        UniTask Show();
        UniTask Hide();
    }
}