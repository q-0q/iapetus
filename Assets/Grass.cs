using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{

    private const float MaximumRotation = 50f;
    private const float PlayerRotationDistance = 6f;
    private const float PlayerRotationExponent = 4f;
    private const float SpringAmount = 2f;

    private const float MaximumTranslation = 0f;

    private Transform _mesh;
    
    // Start is called before the first frame update
    void Start()
    {
        _mesh = transform.Find("Mesh");
    }

    // Update is called once per frame
    void Update()
    {
        return;
        var playerVirtualPosition = PlayerFsm.Singleton.transform.position + PlayerFsm.Singleton.transform.forward * Mathf.Lerp(0, 0.5f, Mathf.InverseLerp(0, 15f, PlayerFsm.Singleton.GetMomentum()));
        var playerToGrass = playerVirtualPosition - transform.position;
        var weight = Mathf.InverseLerp(PlayerRotationDistance, 0, playerToGrass.magnitude);
        weight = Mathf.Pow(weight, PlayerRotationExponent);
        var euler = Quaternion.Euler(new Vector3(-playerToGrass.z, 0, playerToGrass.x) * (MaximumRotation * weight));

        var LerpSpeed = Mathf.Lerp(0.001f, 1f, Mathf.InverseLerp(0.5f, 1f, weight));
        _mesh.rotation = euler;
        _mesh.position = transform.position + (-playerToGrass * (MaximumTranslation * weight));
    }
}
