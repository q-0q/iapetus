using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BitSystem : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject prefab;
    public int initialSize = 20;
    public bool canExpand = true;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    public static BitSystem Singleton;

    void Awake()
    {
        Singleton = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            InstantiatePrefabInPool();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < 30; i++)
            {
                SpawnFromPool(PlayerFsm.Singleton.transform.position + Vector3.up * 2f);
            }
        }
    }

    GameObject InstantiatePrefabInPool()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
        return obj;
    }

    public GameObject SpawnFromPool(Vector3 position)
    {
        if (poolQueue.Count == 0)
        {
            if (canExpand)
            {
                InstantiatePrefabInPool();
            }
            else
            {
                return null;
            }
        }

        GameObject obj = poolQueue.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = Random.rotation;
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}