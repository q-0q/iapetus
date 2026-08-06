using UnityEngine;

public class Giraffe : MonoBehaviour
{

    private Transform _mesh;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _mesh = transform.Find("Mesh");
    }

    // Update is called once per frame
    void Update()
    {
        _mesh.localPosition = Vector3.up * (Mathf.Sin(Time.time * 2f) * 0.4f);
        _mesh.localRotation *= Quaternion.Euler(0f, Time.deltaTime * 130f, 0f);
    }
}
