using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint spawnInstance;

    void Awake()
    {
        if (spawnInstance != null)
        {
            Destroy(this);
        }
        spawnInstance = this;
    }
}
