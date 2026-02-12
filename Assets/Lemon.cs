using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lemon : MonoBehaviour
{

    private Transform _lemonMeshY;
    private Transform _lemonMeshLocalY;
    
    // Start is called before the first frame update
    void Start()
    {
        _lemonMeshY = transform.Find("Mesh").Find("LemonY");
        _lemonMeshLocalY = transform.Find("Mesh").Find("LemonY").Find("LemonLocalY");
    }

    // Update is called once per frame
    void Update()
    {
        _lemonMeshLocalY.rotation *= Quaternion.Euler(0, Time.deltaTime * 100f, 0);
        _lemonMeshY.rotation *= Quaternion.Euler(0, Time.deltaTime * 100f, 0);
    }
}
