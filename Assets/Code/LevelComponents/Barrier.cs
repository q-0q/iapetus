using System;
using System.Collections;
using Code.Misc;
using UnityEngine;
using UnityEngine.Serialization;

public class Barrier : MonoBehaviour
{
    private Animator _animator;

    public Transform curtain;
    private Material _curtainMaterial;
    private Renderer _curtainRenderer;
    private Collider _curtainCollider;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _curtainCollider = curtain.GetComponent<Collider>();
        _curtainRenderer = curtain.GetComponent<Renderer>();
        _curtainMaterial = _curtainRenderer.material;
        _curtainMaterial.SetVector("_WorldspaceOrigin", transform.position);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) DoOpen();
    }

    private void DoOpen()
    {
        
        _curtainMaterial.SetFloat("_OpeningWeight", 0f);
        _curtainRenderer.enabled = true;
        
        Util.ReplaceAnimatorTrigger(_animator, "Opening");
        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            
            
            var t = 0f;
            var d = 0.5f;
            while (t < d)
            {
                // var w = t > d * 0.5f ? Mathf.InverseLerp(d, d * 0.5f, t) : Mathf.InverseLerp(0f, d * 0.5f, t);
                var w = Util.SmoothLerp01(t / d);
                _curtainMaterial.SetFloat("_GlowWeight", w);
                t += Time.deltaTime;
                yield return null;
            }
            
            yield return new WaitForSeconds(0.2f);
            
            _curtainCollider.enabled = false;
            t = 0f;
            d = 3.5f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _curtainMaterial.SetFloat("_OpeningWeight", w);
                t += Time.deltaTime;
                yield return null;
            }
            _curtainRenderer.enabled = false;
        }
    }
}
