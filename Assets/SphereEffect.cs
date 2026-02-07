using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereEffect : MonoBehaviour
{
    private float _age;
    private Material _material;
    private const float MaxAge = 1f;
    public Vector3 initialScale = Vector3.one;
    public float finalScale = 1.25f;
    public float timeToFinalScale = 0.75f;
    public float ageMultiplier = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = initialScale;
        _age = 0;
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        var position = Camera.main.transform.position;
        var transformPosition = Vector3.Lerp(transform.position, position, 3f / Vector3.Distance(transform.position, position));
        _material.SetVector("_PlayerPosition", transformPosition);
        transform.localScale = Vector3.Lerp(initialScale, initialScale * finalScale, Mathf.InverseLerp(0, timeToFinalScale, _age));
        _material.SetFloat("_Age", _age);
        _age += Time.deltaTime * ageMultiplier;
        if (_age >= MaxAge)
        {
            Destroy(gameObject);
        }
    }

    public void SetConfig(Vector3 initialScale, float finalScale, float ageMultiplier, float distanceOffset)
    {
        this.initialScale = initialScale;
        this.finalScale = finalScale;
        this.ageMultiplier = ageMultiplier;
        GetComponent<Renderer>().material.SetFloat("_DistanceOffset", distanceOffset);
    }
}
