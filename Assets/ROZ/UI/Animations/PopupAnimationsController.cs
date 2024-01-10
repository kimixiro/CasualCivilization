using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace ROZ.UI.Animations
{
    public class PopupAnimationsController
        : BasePopupAnimationsController
    {
        [Serializable]
        public struct AnimationTriggerData
        {
            [SerializeField] 
            private float _length;
            [SerializeField] 
            private string _triggerKey;

            public float Length => _length;
            public string TriggerKey => _triggerKey;
        }
        [SerializeField] 
        private Animator _animator;
        
        [SerializeField] 
        private AnimationTriggerData _showTrigger;
        [SerializeField] 
        private AnimationTriggerData _closeTrigger;

        private CancellationTokenSource _animationTokeSource;

        private void Awake()
        {
            Assert.IsNotNull(_animator);
        }

        private async UniTask PlayAnimation
        (
            AnimationTriggerData triggerData, 
            CancellationToken token = default
        )
        {
            _animationTokeSource?.Dispose();
            _animationTokeSource = CancellationTokenSource
                .CreateLinkedTokenSource(token);

            var length = TimeSpan.FromSeconds(triggerData.Length);

            _animator.SetTrigger(triggerData.TriggerKey);

            await UniTask.Delay(length, cancellationToken: _animationTokeSource.Token);
        }

        public override async UniTask PlayShowAnimation(Action<BasePopupAnimationsController> onCompleted = null, 
            CancellationToken token = default)
        {
            try
            {
                await PlayAnimation(_showTrigger, token);
            }
            finally
            {
                onCompleted?.Invoke(this);
            }
        }

        public override async UniTask PlayCloseAnimation(Action<BasePopupAnimationsController> onCompleted = null, 
            CancellationToken token = default)
        {
            try
            {
                await PlayAnimation(_closeTrigger, token);
            }
            finally
            {
                onCompleted?.Invoke(this);
            }
        }
    }
}