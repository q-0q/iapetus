using UnityEngine;
using UnityEngine.Splines;

public class BoostSpline : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var curveLength = GetComponent<SplineContainer>().Spline.GetLength();
        GetComponent<Renderer>().material.SetFloat("_SplineLength", curveLength);
        GetComponent<Renderer>().material.SetFloat("_TimeOffset", Random.Range(0, 5f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
