using System;
using UniRx;

namespace CasualCivilization.GameplayScene.Provinces
{
    public class ProvinceCellModel 
        : IDisposable
    {
        public CubeIndex Index { get; }

        public ReactiveProperty<int> PlayerIndex { get; }
            = new();

        public ProvinceCellModel(CubeIndex index, int playerIndex = -1)
        {
            Index = index;
            PlayerIndex.Value = playerIndex;
        }

        public void Dispose()
        {
            PlayerIndex?.Dispose();
        }
    }
}