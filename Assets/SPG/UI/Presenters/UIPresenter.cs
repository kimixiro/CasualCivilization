using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using SPG.MVP;
using SPG.MVP.Options;
using UnityEngine;
using VContainer;
using Assert = UnityEngine.Assertions.Assert;

namespace SPG.UI.Presenters
{
    public class UIPresenter
        : BasePresenter
    {
        [SerializeField] private UILayerPresenter _layerPrefab;
        [SerializeField] private RectTransform _layersContainer;

        private Dictionary<UILayer, UILayerPresenter> _layers;

        private IObjectResolver _resolver;

        private void Awake()
        {
            SetupLayers();
        }

        private UILayerPresenter CreateLayer(UILayer layer)
        {
            var presenter = Instantiate(_layerPrefab, _layersContainer);

            presenter.name = $"[{layer}_Layer]";

            return presenter;
        }

        private void SetupLayers()
        {
            _layers = new()
            {
                { UILayer.HUD, CreateLayer(UILayer.HUD) },
                { UILayer.Default, CreateLayer(UILayer.Default) },
                { UILayer.Overlay, CreateLayer(UILayer.Overlay) },
            };
        }

        private void OnPopupHidden([NotNull] IShowable showable)
        {
            Assert.IsNotNull(showable);

            showable.Hidden -= OnPopupHidden;
            
            var popup = (BasePopupPresenter) showable;
            
            Destroy(popup.gameObject);
        }

        private TPresenter CreatePopup<TPresenter>(UILayer layer = UILayer.Default)
            where TPresenter : BasePopupPresenter
        {
            var popup = _resolver.Resolve<TPresenter>();

            popup.Hidden += OnPopupHidden;
            
            _layers[layer].AddPopup(popup);
            
            return popup;
        }
        
        [Inject]
        public void Construct([NotNull] IObjectResolver resolver)
        {
            Assert.IsNotNull(resolver);
            
            _resolver = resolver;
        }

        public bool IsPopupShown<TPopupPresenter>(UILayer layer = UILayer.Default)
            where TPopupPresenter : BasePopupPresenter
        {
            return _layers[layer].IsPopupShown<TPopupPresenter>();
        }

        public bool TryGetShownPopup<TPopupPresenter>(out TPopupPresenter presenter, UILayer layer = UILayer.Default)
            where TPopupPresenter : BasePopupPresenter
        {
            return _layers[layer].TryGetShownPopup(out presenter);
        }
        
        [NotNull]
        public TPopupPresenter GetShownPopup<TPopupPresenter>(UILayer layer = UILayer.Default)
            where TPopupPresenter : BasePopupPresenter
        {
            return _layers[layer].GetShownPopup<TPopupPresenter>();
        }
        
        public async UniTask<bool> TryHidePopup<TPopupPresenter>(UILayer layer = UILayer.Default)
            where TPopupPresenter : BasePopupPresenter
        {
            if (!_layers[layer].TryGetShownPopup<TPopupPresenter>(out var presenter))
            {
                return false;
            }
            await presenter.Hide();

            return true;
        }

        public async UniTask HidePopup<TPopupPresenter>(UILayer layer = UILayer.Default)
            where TPopupPresenter : BasePopupPresenter
        {
            var popup = GetShownPopup<TPopupPresenter>(layer);

            await popup.Hide();
        }
        
        public async UniTask<TPopupPresenter> ShowPopup<TPopupPresenter>
        (
            UILayer layer = UILayer.Default,
            bool isSingle = true,
            bool isMoveToTop = true
        )
            where TPopupPresenter : BasePopupPresenter
        {
            if (isSingle && TryGetShownPopup<TPopupPresenter>(out var presenter, layer))
            {
                if (isMoveToTop)
                {
                    _layers[layer].MoveToTop<TPopupPresenter>();
                }
                return presenter;
            }
            var popup = CreatePopup<TPopupPresenter>(layer);

            await popup.Show();
            
            return popup;
        }

        public async UniTask<TPopupPresenter> ShowPopup<TPopupPresenter, TPresenterData>
        (
            TPresenterData data, 
            UILayer layer = UILayer.Default,
            bool isSingle = true,
            bool isMoveToTop = true
        )
            where TPopupPresenter : BasePopupPresenter, IHasData<TPresenterData>
            where TPresenterData : struct
        {
            if (isSingle && TryGetShownPopup<TPopupPresenter>(out var presenter, layer))
            {
                if (isMoveToTop)
                {
                    _layers[layer].MoveToTop<TPopupPresenter>();
                }
                return presenter;
            }
            var popup = CreatePopup<TPopupPresenter>(layer);

            popup.SetData(data);
            
            await popup.Show();
            
            return popup;
        }

        public async UniTask ShowLayer(UILayer layer = UILayer.Default)
        {
            await _layers[layer].Show();
        }

        public async UniTask HideLayer(UILayer layer = UILayer.Default)
        {
            await _layers[layer].Hide();
        }
    }
}