using System.Collections.Generic;
using UnityEngine;

namespace Video
{
    public class WallController : MonoBehaviour
    {
        public LayerMask wallLayer;
        public List<WallDisplay> allWallDisplays; // Assign all WallDisplay instances here in the inspector
        public float maxCheckDistance = 10f; // Set this to an appropriate value for your game

        private void Awake()
        {
            // Assuming this gets called when a WallDisplay is activated
            RefreshWalls();
        }

        public void RefreshWalls()
        {
            // First, activate all wall segments in all WallDisplays
            foreach (var wallDisplay in allWallDisplays)
            {
                wallDisplay.ActivateAllWallSegments();
            }

            // Now, check each WallDisplay against every other one
            foreach (var wallDisplay in allWallDisplays)
            {
                wallDisplay.CheckAndDeactivateWalls(wallLayer, maxCheckDistance);
            }
        }
    }
}