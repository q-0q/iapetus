using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class BellController : MonoBehaviour
{
    private Interactable _interactable;
    private Animator _animator;
    public string persistentEvent;
    private bool rung;
    public Transform bellMesh;
    private Material _bellMaterial;

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        var saveData = SaveSystem.LoadSaveData(0);
        rung = saveData.persistentEvents.Contains(persistentEvent);
        _bellMaterial = bellMesh.GetComponent<Renderer>().material;

    }

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        Util.ReplaceAnimatorTrigger(_animator, "Idle");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    private void OnInteracted()
    {
        // if (rung) return;
        
        Util.ReplaceAnimatorTrigger(_animator, "Ring");
        rung = true;

        StartCoroutine(Coroutine());

        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(2.25f);
            transform.DOShakePosition(3f, 0.2f, 30);

            float t = 0;
            float duration = 1f;
            while (t < duration)
            {
                var value = Mathf.Lerp(0f, 1f, (duration - t) / duration);
                _bellMaterial.SetFloat("_GlowWeight", value);
                t += Time.deltaTime;
                yield return null;
            }
        }
        
    }
}
