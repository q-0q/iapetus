using System;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public class LemonDoor : MonoBehaviour
{
    public int count = 3;
    private const float lightPlacementRadius = 2.5f;
    private const float lightPlacementSeparationAngle = 25f;
    private Animator _animator;
    public Transform lightHolder;

    private List<GameObject> _lights;

    private void Awake()
    {
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
        
    }

    private void UpdateLightMaterial(SaveSystem.SaveData _)
    {
        var c = SaveSystem.LoadCachedSaveData().lemonCollections.Count;
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
        if (Input.GetKeyDown(KeyCode.P)) DoOpen();
    }

    private void OnEnable()
    {
        SaveSystem.OnSaveDataUpdated += UpdateLightMaterial;
    }

    private void OnDisable()
    {
        SaveSystem.OnSaveDataUpdated -= UpdateLightMaterial;
    }

    void DoOpen()
    {
        Util.ReplaceAnimatorTrigger(_animator, "Opening");
    }
}
