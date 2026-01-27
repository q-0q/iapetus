using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    private const float MaximumRotation = 30f;
    private const float PlayerRotationDistance = 4f;
    private const float PlayerRotationExponent = 3f;

    private const float MaximumTranslation = 1f;

    private Transform _mesh;
    
    // Start is called before the first frame update
    void Start()
    {
        _mesh = transform.Find("Mesh");
    }

    // Update is called once per frame
    void Update()
    {
        var playerToGrass = PlayerFsm.Singleton.transform.position - transform.position;
        var weight = Mathf.InverseLerp(PlayerRotationDistance, 0, playerToGrass.magnitude);
        weight = Mathf.Pow(weight, PlayerRotationExponent);
        
        var euler = new Vector3(-playerToGrass.z, 0, playerToGrass.x) * (MaximumRotation * weight);
        _mesh.rotation = Quaternion.Euler(euler);
        
        _mesh.position = transform.position + (-playerToGrass * (MaximumTranslation * weight));
    }
}
