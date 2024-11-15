using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCullingAdjuster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];
        distances[7] = 0;
        camera.layerCullDistances = distances;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
