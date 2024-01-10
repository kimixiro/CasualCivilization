using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ROZ.MVP;
using ROZ.MVP.Options;
using UnityEngine;
using UnityEngine.Assertions;

namespace CasualCivilization.GameplayScene.Grid
{
    public class GridPresenter 
        : BasePresenter
        , IHasModel<GridModel>
    {
        private readonly Dictionary<CubeIndex, GridCellPresenter> _cells = new();

        [SerializeField] 
        private Transform _cellContainer;
        
        public GridModel Model { get; private set; }

        public async UniTask SetupGrid()
        {
            foreach (var girdCellModel in Model.Cells)
            {
                var cellPresenter = Instantiate(girdCellModel.Value.Config.CellPrefab, _cellContainer);
                
                _cells.Add(girdCellModel.Key, cellPresenter);

                await UniTask.DelayFrame(1);
            }
        }

        public void SetModel(GridModel model)
        {
            Assert.IsNotNull(model);
            Assert.IsNull(Model);

            Model = model;
        }

        public void UnsetModel()
        {
            Model?.Dispose();
            
            Model = null;
        }
    }
}