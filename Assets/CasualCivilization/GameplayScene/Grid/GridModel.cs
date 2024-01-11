using System;
using System.Diagnostics.CodeAnalysis;
using SPG.MVP;
using SPG.MVP.Options;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace CasualCivilization.GameplayScene.Grid
{
    [Serializable]
    public class GridConfig
    {
        
    }

    public class GridCellPresenter
        : BasePresenter
        , IHasModel<GridModel>
        , IPointerClickHandler
    {
        [SerializeField] 
        private Transform _entityPlacePoint;

        public GridModel Model { get; }

        public Vector3 EntityPlacePoint => _entityPlacePoint.position;

        public void OnPointerClick(PointerEventData eventData)
        {
            // Model.
        }
        
        private void Awake()
        {
            Assert.IsNotNull(_entityPlacePoint);
        }

        public void SetModel(GridModel model)
        {
            
        }

        public void UnsetModel()
        {
        }
    }

    public class GirdCellModel : IDisposable
    {
        public event Action<GirdCellModel> Clicked;
        
        private readonly BoolReactiveProperty _isWalkable = new();
        public IReadOnlyReactiveProperty<bool> IsWalkable => _isWalkable;

        public GridCellConfig Config { get; }
        public CubeIndex Position { get; }

        public GirdCellModel
        (
            [NotNull] GridCellConfig config, 
            CubeIndex position
        )
        {
            Config = config;
            Position = position;

            _isWalkable.Value = config.IsWalkable;
        }

        public void Dispose()
        {
            _isWalkable?.Dispose();
        }
    }

    public class GridModel : IDisposable
    {
        private readonly ReactiveDictionary<CubeIndex, GirdCellModel> _cells = new();
        
        public IReadOnlyReactiveDictionary<CubeIndex, GirdCellModel> Cells => _cells;

        public void BuildGrid()
        {
        }

        public void Dispose()
        {
        }
    }
}