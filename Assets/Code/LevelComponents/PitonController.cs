using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitonController : MonoBehaviour
{

    private Transform _rotationParent;
    public bool Rotate;
    
    // Start is called before the first frame update
    void Start()
    {
        _rotationParent = transform.Find("Mesh").Find("RotationHolder");
        Rotate = false;
    }

    // Update is called once per frame
    void Update()
    {
        var currentXRotation = _rotationParent.localRotation.eulerAngles.x;
        var newX = Mathf.Lerp(currentXRotation, Rotate ? 15f : 0, Time.deltaTime * 10f);
        _rotationParent.localRotation = Quaternion.Euler(newX, 0, 0);
    }
}
