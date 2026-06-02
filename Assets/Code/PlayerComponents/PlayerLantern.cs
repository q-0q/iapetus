using System;
using UnityEngine;

public class PlayerLantern : MonoBehaviour
{
    private GameObject _child;

    private void Awake()
    {
        _child = transform.Find("Child").gameObject;
        _child.SetActive(false);
    }

    private void OnEnable()
    {
        SceneDarknessIndicator.OnPlayerLanternActivated += OnLanternActivated;
    }

    private void OnDisable()
    {
        SceneDarknessIndicator.OnPlayerLanternActivated -= OnLanternActivated;
    }

    private void OnLanternActivated()
    {
        _child.SetActive(true);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // float rotationSpeed = 100f;
        float posLerpSpeed = 3f;
        transform.position = Vector3.Lerp(transform.position, PlayerFsm.Singleton.transform.position, posLerpSpeed * Time.deltaTime);
        // transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
