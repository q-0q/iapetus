using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboTrigger : MonoBehaviour
{
    private float _age;
    private Material _material;
    private const float MaxAge = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _age = 0;
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        var position = Camera.main.transform.position;
        var transformPosition = Vector3.Lerp(transform.position, position, 3f / Vector3.Distance(transform.position, position));
        _material.SetVector("_PlayerPosition", transformPosition);
        
        _material.SetFloat("_Age", _age);
        _age += Time.deltaTime;
        if (_age >= MaxAge)
        {
            print("destroying");
            Destroy(gameObject);
        }
    }
}
