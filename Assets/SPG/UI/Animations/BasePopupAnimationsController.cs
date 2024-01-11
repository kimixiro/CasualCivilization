using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SPG.UI.Animations
{
    public abstract class BasePopupAnimationsController
        : MonoBehaviour
    {
        public abstract UniTask PlayShowAnimation(Action<BasePopupAnimationsController> onCompleted = null, 
            CancellationToken token = default);
        
        public abstract UniTask PlayCloseAnimation(Action<BasePopupAnimationsController> onCompleted = null, 
            CancellationToken token = default);
    }
}