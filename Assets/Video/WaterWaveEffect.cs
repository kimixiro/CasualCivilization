using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaveEffect : MonoBehaviour
{
    public List<Transform> waterTiles; // List to hold all water tiles
    public float waveAmplitude = 0.5f; // Height of the wave
    public float waveFrequency = 1f; // Speed of the wave
    public float waveLength = 2f; // Distance between wave peaks

    private void Update()
    {
        // Loop through each water tile
        foreach (Transform tile in waterTiles)
        {
            // Calculate wave effect based on position and time to create a moving wave
            float phase = Time.time * waveFrequency + tile.position.x * waveLength + tile.position.z * waveLength;
            float waveHeight = Mathf.Sin(phase) * waveAmplitude;

            // Apply the calculated wave height to the tile's Y position
            tile.position = new Vector3(tile.position.x, waveHeight, tile.position.z);
        }
    }
}
