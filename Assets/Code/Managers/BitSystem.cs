using System;
using System.Collections;
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

    private const string DecrementEventPath = "event:/BitCollect";
    public static event Action OnBitsDecremented;

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

    public void RemoveBits(int amount)
    {
        StartCoroutine(Coroutine());
        
        IEnumerator Coroutine()
        {
            for (int i = 0; i < amount; i+=10)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference(DecrementEventPath), PlayerFsm.Singleton.gameObject);
                SaveSystem.RemoveBit(10, 0);
                OnBitsDecremented?.Invoke();
                yield return new WaitForSeconds(Random.Range(0.02f, 0.04f));
            }
        }
    }
}