using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteAlways]
public class PrefabGrid : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float sizeX = 5f;
    [SerializeField] private float sizeZ = 5f;
    [SerializeField] private float density = 1f;
    [SerializeField] private float maxPositionOffsetX = 1f;
    [SerializeField] private float minPositionOffsetX = -1f;
    [SerializeField] private float maxPositionOffsetZ = 1f;
    [SerializeField] private float minPositionOffsetZ = -1f;
    [SerializeField] private Vector3 positionOffsetMultiplier = Vector3.one;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float maxScaleYMod = 1.5f;
    [SerializeField] private float minRotation = 0f;
    [SerializeField] private float maxRotation = 0f;
    
    
    // Method with parameter that only works in editor mode.
    [SerializeField, ButtonInvoke(nameof(Bake), 1.1f, ButtonInvoke.DisplayIn.EditMode)] private bool bake;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Bake();
    }

    private void Bake()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        
        var x = sizeX;
        var z = sizeZ;
        for (float i = -x * 0.5f + transform.position.x; i < x * 0.5f + transform.position.x; i += density)
        {
            for (float j = -z * 0.5f + transform.position.z; j < z * 0.5f + transform.position.z; j += density)
            {
                var xRange = UnityEngine.Random.Range(minPositionOffsetX, maxPositionOffsetX);
                var ZRange = UnityEngine.Random.Range(minPositionOffsetZ, maxPositionOffsetZ);
                var pos = new Vector3(i + xRange, transform.position.y,  j + ZRange);
                pos = new Vector3(pos.x * positionOffsetMultiplier.x, pos.y * positionOffsetMultiplier.y,
                    pos.z * positionOffsetMultiplier.z);
                
                #if UNITY_EDITOR
                var obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                obj.transform.position = pos;
                #else
                var obj = Instantiate(prefab, pos, Quaternion.identity, transform);
                #endif
                
                obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(minRotation, maxRotation), 0f);
                var s = UnityEngine.Random.Range(minScale, maxScale);
                obj.transform.SetParent(transform);
                obj.transform.localScale = new Vector3(s, s * UnityEngine.Random.Range(1, maxScaleYMod), s);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(sizeX, 1, sizeZ));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
