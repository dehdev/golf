using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] List<GameObject> m_SpawnPoints;

    static SpawnPointManager s_Instance;

    public static SpawnPointManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<SpawnPointManager>();
            }

            return s_Instance;
        }
    }

    void OnDestroy()
    {
        s_Instance = null;
    }

    public GameObject ConsumeNextSpawnPoint()
    {
        if (m_SpawnPoints.Count == 0)
        {
            return null;
        }

        var toReturn = m_SpawnPoints[m_SpawnPoints.Count - 1];
        m_SpawnPoints.RemoveAt(m_SpawnPoints.Count - 1);
        return toReturn;
    }
}