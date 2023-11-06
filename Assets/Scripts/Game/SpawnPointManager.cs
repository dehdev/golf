using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    public static SpawnPointManager Instance { get; private set; }

    private GameObject spawnPoint;

    private void Awake()
    {
        Instance = this;
        spawnPoint = GameObject.FindWithTag("SpawnPoint");
    }
    public Vector3 GetSpawnPoint()
    {
        if (spawnPoint != null)
        {
            return spawnPoint.transform.position;
        }
        return Vector3.zero;
    }
}