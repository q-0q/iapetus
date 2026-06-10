using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;

    [SerializeField]
    private bool _keepWorldUp = false;
    
    [SerializeField]
    private bool _keepCameraUp = false;
    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (_keepWorldUp)
        {
            transform.LookAt(new Vector3(_camera.transform.position.x, transform.position.y, _camera.transform.position.z), Vector3.up);
        }
        else if (_keepCameraUp)
        {
            transform.LookAt(_camera.transform, _camera.transform.up);
        }
        else
        {
            transform.LookAt(_camera.transform, Vector3.up);
        }
    }
}
