using UnityEngine;

namespace Video
{
    public class WallDisplay : MonoBehaviour
    {
        public GameObject[] wallSegments; // Assign the child wall segments in the prefab in the inspector

        // Directions to check for neighboring wall segments in a hex grid
        private readonly Vector3[] directions = new Vector3[]
        {
            new Vector3(1, 0, 0), // Right
            new Vector3(-1, 0, 0), // Left
            new Vector3(0.5f, 0, Mathf.Sqrt(3) / 2), // Upper-right
            new Vector3(-0.5f, 0, Mathf.Sqrt(3) / 2), // Upper-left
            new Vector3(0.5f, 0, -Mathf.Sqrt(3) / 2), // Lower-right
            new Vector3(-0.5f, 0, -Mathf.Sqrt(3) / 2), // Lower-left
        };

        public void CheckAndDeactivateWalls(LayerMask wallLayer, float maxDistance)
        {
            // Disable all colliders of this WallDisplay to prevent self-hits
            SetWallSegmentsCollidersEnabled(false);

            for (int i = 0; i < directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                RaycastHit hit;
                // Start slightly above the ground to avoid self-collision
                Vector3 start = transform.position + transform.up * 0.5f;
                bool isHit = Physics.Raycast(start, dir, out hit, maxDistance, wallLayer);

                // Debugging: Draw the ray in the scene view
                Debug.DrawRay(start, dir * (isHit ? hit.distance : maxDistance), isHit ? Color.red : Color.green, 1.0f);

                if (isHit)
                {
                    WallDisplay hitWallDisplay = hit.collider.GetComponentInParent<WallDisplay>();
                    if (hitWallDisplay != null && hitWallDisplay != this)
                    {
                        // Deactivate the wall segment in this WallDisplay
                        DeactivateWallSegment(i);

                        // Find the index of the hit segment in the hit WallDisplay and deactivate it
                        int hitSegmentIndex = hitWallDisplay.GetSegmentIndexFromDirection(-dir);
                        if (hitSegmentIndex != -1)
                        {
                            hitWallDisplay.DeactivateWallSegment(hitSegmentIndex);
                        }
                    }
                }
            }

            // Re-enable all colliders of this WallDisplay after checks
            SetWallSegmentsCollidersEnabled(true);
        }

        private void SetWallSegmentsCollidersEnabled(bool enabled)
        {
            foreach (var segment in wallSegments)
            {
                Collider collider = segment.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = enabled;
                }
            }
        }

        private void ActivateWallSegment(int index)
        {
            if (index >= 0 && index < wallSegments.Length)
            {
                wallSegments[index].SetActive(true);
            }
        }
        
        public void ActivateAllWallSegments()
        {
            foreach (var segment in wallSegments)
            {
                segment.SetActive(true); // Activate each segment
            }
        }

        private void DeactivateWallSegment(int index)
        {
            if (index >= 0 && index < wallSegments.Length)
            {
                wallSegments[index].SetActive(false);
            }
        }

        private int GetSegmentIndexFromDirection(Vector3 direction)
        {
            for (int i = 0; i < directions.Length; i++)
            {
                // Use TransformDirection to account for the wall's rotation
                if (Vector3.Equals(transform.TransformDirection(directions[i]).normalized, direction.normalized))
                {
                    return i;
                }
            }

            return -1; // Return -1 if the direction does not match any segment
        }
    }
}
