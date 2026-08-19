using System;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public class LemonDoor : MonoBehaviour
{
    public int count = 3;
    public string persistentEvent = "-lemon-door";
    private const float lightPlacementRadius = 2.5f;
    private const float lightPlacementSeparationAngle = 25f;
    private Animator _animator;
    public Transform lightHolder;

    private List<GameObject> _lights;
    private TriggerProxy _triggerProxy;

    private void Awake()
    {
        _triggerProxy = GetComponentInChildren<TriggerProxy>();
        _animator = GetComponentInChildren<Animator>();
        var lightPrefab = Resources.Load("Prefab/LemonDoorLight") as GameObject;
        _lights = new List<GameObject>();
        
        for (int i = 0; i < count; i++)
        {
            var angle = lightPlacementSeparationAngle * (i - (count - 1) * 0.5f);
            var offset = Quaternion.Euler(0f, 0f, angle) * Vector3.up * lightPlacementRadius;
            var lightObject = Instantiate(lightPrefab, lightHolder);
            lightObject.transform.SetLocalPositionAndRotation(offset, Quaternion.identity);
            _lights.Add(lightObject);
        }
        
        UpdateLightMaterial(SaveSystem.LoadCachedSaveData());
        if (SaveSystem.GetPersistentEventCompleted(persistentEvent))
        {
            Util.ReplaceAnimatorTrigger(_animator, "Open");
            _triggerProxy.gameObject.SetActive(false);
        }
        
    }

    private void UpdateLightMaterial(SaveSystem.SaveData _)
    {
        var c = Math.Min(SaveSystem.LoadCachedSaveData().lemonCollections.Count, _lights.Count);
        for (int i = 0; i < c; i++)
        {
            _lights[i].GetComponentInChildren<Renderer>().material.SetFloat("_Weight", 1f);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        SaveSystem.OnSaveDataUpdated += UpdateLightMaterial;
        _triggerProxy.OnTriggerProxyStay += OnTriggerProxyStay;
    }

    private void OnTriggerProxyStay(Collider obj)
    {
        var c = SaveSystem.LoadCachedSaveData().lemonCollections.Count;
        if (c < count) return;
        SaveSystem.WritePersistentEvent(persistentEvent);
        Util.ReplaceAnimatorTrigger(_animator, "Opening");
        _triggerProxy.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SaveSystem.OnSaveDataUpdated -= UpdateLightMaterial;
        _triggerProxy.OnTriggerProxyStay -= OnTriggerProxyStay;
    }

}
