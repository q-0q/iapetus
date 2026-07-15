using System;
using Code.Misc;
using UnityEngine;

public class LeylineDoor : MonoBehaviour
{
    public string requiredNode;
    private Animator _animator;
    private Collider _collider;
    public Renderer glowRenderer;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        TryGetComponent(out Collider _collider);
        _collider.enabled = false;
        

        if (SaveSystem.GetPersistentEventCompleted(requiredNode))
        {
            Util.ReplaceAnimatorTrigger(_animator, "Open");
            glowRenderer.material.SetFloat("_GlowWeight", 1f);
        }
    }

    private void OnEnable()
    {
        SaveSystem.OnSaveDataUpdated += OnSaveDataUpdated;
    }

    private void OnDisable()
    {
        SaveSystem.OnSaveDataUpdated -= OnSaveDataUpdated;
    }

    private void OnSaveDataUpdated(SaveSystem.SaveData obj)
    {
        if (SaveSystem.GetTerminalNode(requiredNode))
        {
            glowRenderer.material.SetFloat("_GlowWeight", 1f);
            _collider.enabled = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        throw new NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
