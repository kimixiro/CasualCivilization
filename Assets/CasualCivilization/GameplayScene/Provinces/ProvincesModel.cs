using System;
using System.Diagnostics.CodeAnalysis;
using CasualCivilization.GameplayScene.Grid;
using SPG.Logging;
using UniRx;
using UnityEngine.Assertions;

namespace CasualCivilization.GameplayScene.Provinces
{
    public class ProvincesModel 
        : IDisposable
    {
        private readonly CompositeDisposable _disposable;
        
        private readonly GridModel _gridModel;
        
        private readonly ReactiveDictionary<CubeIndex, ProvinceCellModel> _provincesCells = new();
        
        public IReadOnlyReactiveDictionary<CubeIndex, ProvinceCellModel> ProvincesCells => _provincesCells;

        private void OnCellRemoved(DictionaryRemoveEvent<CubeIndex, GirdCellModel> eventData)
        {
            _provincesCells.Remove(eventData.Key);
        }

        private void OnCellAdded(DictionaryAddEvent<CubeIndex, GirdCellModel> eventData)
        {
            var model = new ProvinceCellModel(eventData.Key);
            
            _provincesCells.Add(eventData.Key, model);
        }
        
        public ProvincesModel
        (
            [NotNull] GridModel gridModel
        )
        {
            Assert.IsNotNull(gridModel);

            _disposable = new();
            
            _gridModel = gridModel;

            // _gridModel.Cells
            //     .ObserveAndSubscribe
            //     (
            //         OnCellAdded,
            //         OnCellRemoved
            //     )
            //     .AddTo(_disposable);
        }
            
        public void ChangeOwner(CubeIndex index, int ownerIndex)
        {
            if (!_provincesCells.TryGetValue(index, out var model))
            {
                Logger.LogError(nameof(ProvincesModel), nameof(ChangeOwner), $"Can't find cell by: {index}");
                
                return;
            }
            model.PlayerIndex.Value = ownerIndex;
        }

        public void Dispose()
        {
            _provincesCells?.Dispose();
        }
    }
}