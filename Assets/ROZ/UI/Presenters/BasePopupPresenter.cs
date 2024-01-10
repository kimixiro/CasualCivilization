using System;
using Cysharp.Threading.Tasks;
using ROZ.MVP;
using ROZ.UI.Animations;
using UnityEngine;

namespace ROZ.UI.Presenters
{
    public abstract class BasePopupPresenter
        : BasePresenter
        , IShowable
    {
        public event Action<IShowable> Hidden;
        public event Action<IShowable> Showed;

        [SerializeField] private BasePopupAnimationsController _animationsController;
        
        public virtual async UniTask Show()
        {
            if (_animationsController != null)
            {
                await _animationsController.PlayShowAnimation(_ => Showed?.Invoke(this));
                
                return;
            }
            Showed?.Invoke(this);
        }

        public virtual async UniTask Hide()
        {
            if (_animationsController != null)
            {
                await _animationsController.PlayCloseAnimation(_ => Hidden?.Invoke(this));
                
                return;
            }
            Hidden?.Invoke(this);
        }

        public void HideAndForget()
        {
            Hide().Forget();
        }
    }
}