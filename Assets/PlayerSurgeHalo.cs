using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public class PlayerSurgeHalo : MonoBehaviour
{
    
    private Material _material;

    private bool _isChanneling;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChanneling) return;
        
        _material.SetFloat("_Weight", Mathf.Lerp(_material.GetFloat("_Weight"), 0, Time.deltaTime * 2f));
    }
    
    public void StartStartup()
    {
        if (_isChanneling) return;
        StartCoroutine(StartupCoroutine());
        transform.position = PlayerFsm.Singleton.transform.position;
    }

    public void EndStartup()
    {
        _isChanneling = false;
    }
    
    public void StartBreak()
    {
        if (_isChanneling) return;
        StartCoroutine(BreakCoroutine());
        transform.position = PlayerFsm.Singleton.transform.position;
    }
    
    private IEnumerator StartupCoroutine()
    {
        _isChanneling = true;
        var t = 0f;
        var d = 1.25f;

        while (t < d)
        {
            var w = Util.SmoothLerp01(t / d);
            _material.SetFloat("_Weight", w);
            t += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    
    private IEnumerator BreakCoroutine()
    {
        _isChanneling = true;
        var t = 0f;
        var d = 0.15f;

        while (t < d)
        {
            var w = Util.SmoothLerp01(t / d);
            _material.SetFloat("_Weight", w);
            t += Time.deltaTime;
            yield return null;
        }

        _isChanneling = false;
        yield break;
    }
}
