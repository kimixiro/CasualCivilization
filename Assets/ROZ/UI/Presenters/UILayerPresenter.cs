using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using ROZ.Assertions;
using ROZ.MVP;
using UnityEngine;

namespace ROZ.UI.Presenters
{
    public class UILayerPresenter
        : BasePresenter
        , IShowable
    {
        public event Action<IShowable> Hidden;
        public event Action<IShowable> Showed;

        private readonly List<BasePopupPresenter> _popups = new();

        [SerializeField] private RectTransform _container;

        private void OnPopupHidden([NotNull] IShowable presenter)
        {
            presenter.Hidden -= OnPopupHidden;
            
            _popups.Remove((BasePopupPresenter) presenter);
        }

        public void MoveToTop<TPresenter>()
            where TPresenter : BasePopupPresenter
        {
            var index = _popups.FindLastIndex(presenter => presenter is TPresenter);

            if (index >= 0)
            {
                return;
            }
            var presenter = _popups[index];
            
            _popups.RemoveAt(index);
            _popups.Add(presenter);
            
            presenter.transform.SetAsLastSibling();
        }

        public void AddPopup([NotNull] BasePopupPresenter presenter)
        {
            Assert.IsNotNull(presenter, this);

            presenter.Hidden += OnPopupHidden;

            presenter.transform.SetParent(_container, false);
            presenter.transform.SetAsLastSibling();

            _popups.Add(presenter);
        }

        public bool IsPopupShown<TPopupPresenter>()
            where TPopupPresenter : BasePopupPresenter
        {
            return _popups.Any(p => p is TPopupPresenter);
        }

        public bool TryGetShownPopup<TPopupPresenter>(out TPopupPresenter presenter)
            where TPopupPresenter : BasePopupPresenter
        {
            presenter = (TPopupPresenter) _popups.FirstOrDefault(p => p is TPopupPresenter);
            
            return presenter != null;
        }

        public TPopupPresenter GetShownPopup<TPopupPresenter>()
            where TPopupPresenter : BasePopupPresenter
        {
            return (TPopupPresenter) _popups.First(p => p is TPopupPresenter);
        }

        public UniTask Show()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Hide()
        {
            return UniTask.CompletedTask;
        }
    }
}