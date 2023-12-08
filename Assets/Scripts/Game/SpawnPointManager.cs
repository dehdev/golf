using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> SpawnPoints;

    public static SpawnPointManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public List<GameObject> GetSpawnPointsList()
    {
        return SpawnPoints;
    }
}