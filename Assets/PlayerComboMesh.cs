using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComboMesh : MonoBehaviour
{
    private float _age;
    private Material _material;
    private const float MaxAge = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        _age = 0;
        _material = GetComponent<Renderer>().material;
        var transformPosition = PlayerFsm.Singleton.transform.position + (PlayerFsm.Singleton.transform.forward * 2f) +
                                (PlayerFsm.Singleton.transform.up * 2f);
        _material.SetVector("_PlayerPosition", transformPosition);
    }

    // Update is called once per frame
    void Update()
    {
        _material.SetFloat("_Age", _age);
        _age += Time.deltaTime;
        if (_age >= MaxAge)
        {
            print("destroying");
            Destroy(gameObject);
        }
    }
}
