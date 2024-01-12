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
            // First, activate all wall segments in all active WallDisplays
            foreach (var wallDisplay in allWallDisplays)
            {
                if (wallDisplay.gameObject.activeInHierarchy) // Check if the WallDisplay is active in the scene
                {
                    wallDisplay.ActivateAllWallSegments();
                }
            }

            // Now, check each active WallDisplay against every other active WallDisplay
            foreach (var wallDisplay in allWallDisplays)
            {
                if (wallDisplay.gameObject.activeInHierarchy) // Only proceed if the WallDisplay is active
                {
                    wallDisplay.CheckAndDeactivateWalls(wallLayer, maxCheckDistance);
                }
            }
        }

    }
}