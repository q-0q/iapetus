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
    [FormerlySerializedAs("sizeY")] [SerializeField] private float sizeZ = 5f;
    [SerializeField] private float density = 1f;
    [SerializeField] private float maxPositionOffset = 1f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1f;
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
                var pos = new Vector3(i + UnityEngine.Random.Range(-maxPositionOffset, maxPositionOffset), transform.position.y,  j + UnityEngine.Random.Range(-maxPositionOffset, maxPositionOffset));
                
                
                #if UNITY_EDITOR
                var obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
                obj.transform.position = pos;
                #else
                var obj = Instantiate(prefab, pos, Quaternion.identity, transform);
                #endif
                
                obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(minRotation, maxRotation), 0f);
                var s = UnityEngine.Random.Range(minScale, maxScale);
                obj.transform.SetParent(transform);
                obj.transform.localScale = new Vector3(s, s, s);
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
