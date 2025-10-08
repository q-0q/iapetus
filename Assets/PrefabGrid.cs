using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PrefabGrid : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float density = 1f;
    [SerializeField] private float maxPositionOffset = 1f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float minRotation = 0f;
    [SerializeField] private float maxRotation = 0f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        var x = transform.lossyScale.x;
        var z = transform.lossyScale.z;
        for (float i = -x * 0.5f + transform.position.x; i < x * 0.5f + transform.position.x; i += density)
        {
            for (float j = -z * 0.5f + transform.position.z; j < z * 0.5f + transform.position.z; j += density)
            {
                var pos = new Vector3(i + UnityEngine.Random.Range(-maxPositionOffset, maxPositionOffset), transform.position.y,  j + UnityEngine.Random.Range(-maxPositionOffset, maxPositionOffset));
                var obj = Instantiate(prefab, pos, Quaternion.identity);
                obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(minRotation, maxRotation), 0f);
                var s = UnityEngine.Random.Range(minScale, maxScale);
                obj.transform.localScale = new Vector3(s, s, s);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
