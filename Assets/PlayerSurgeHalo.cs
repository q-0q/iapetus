using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public class PlayerSurgeHalo : MonoBehaviour
{
    
    private Renderer _renderer;
    private Material _material;

    private bool _isChanneling;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
        _renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChanneling) return;
        
        _material.SetFloat("_Weight", Mathf.Lerp(_material.GetFloat("_Weight"), 0, Time.deltaTime * 0.75f));
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

        _renderer.enabled = true;
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
        var d = 0.15f;
        var triggerPrefab = Resources.Load("Prefab/Fsm/SphereEffect") as GameObject;
        var triggerPosition = PlayerFsm.Singleton.transform.position;
        var triggerObject = Instantiate(triggerPrefab, triggerPosition,
            Quaternion.identity, null);
        triggerObject.GetComponent<SphereEffect>().SetConfig(Vector3.one * 15f, 1.25f, 0.6f, -4.5f);

        _material.SetFloat("_Weight", 1f);

        yield return new WaitForSeconds(d);
        _isChanneling = false;
        _renderer.enabled = false;
        yield break;
    }
}
