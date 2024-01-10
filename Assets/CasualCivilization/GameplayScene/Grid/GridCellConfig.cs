using UnityEngine;

namespace CasualCivilization.GameplayScene.Grid
{
    public class GridCellConfig
        : ScriptableObject
    {
        [SerializeField] 
        private GridCellPresenter _cellPrefab;

        [SerializeField] 
        private bool _isWalkable;
        
        public GridCellPresenter CellPrefab;
        public bool IsWalkable => _isWalkable;
    }
}