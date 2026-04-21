using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ComboIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var prefab = Resources.Load("Prefab/Fsm/PlayerComboIndicator") as GameObject;
        var radius = 1.5f;
        Vector3 offset = Vector3.up;
        for (int i = 0; i < PlayerFsm.MaxComboLength; i++)
        {
            var position = Quaternion.Euler(0, 360f * i / PlayerFsm.MaxComboLength, 0) * (transform.forward * radius);
            position += offset;
            var obj = Instantiate(prefab, transform);
            obj.transform.localPosition = position;
        }
        transform.SetParent(null);
        OnReset();
    }

    // Update is called once per frame
    void Update()
    {
        var rotationSpeed = 100f;
        if (PlayerFsm.Singleton.GetComboLength() >= PlayerFsm.MaxComboLength) rotationSpeed = 200f;
        transform.Rotate(new Vector3(0, Time.deltaTime * rotationSpeed, 0));
        transform.position = PlayerFsm.Singleton.transform.position;
    }

    private void OnIncrement(int length)
    {
        if (length > PlayerFsm.MaxComboLength) return;
        transform.GetChild(length - 1).GetComponentInChildren<Renderer>().enabled = true;
    }

    private void OnReset()
    {
        for (int i = 0; i < PlayerFsm.MaxComboLength; i++)
        {
            transform.GetChild(i).GetComponentInChildren<Renderer>().enabled = false;
        }    
    }

    private void OnEnable()
    {
        PlayerFsm.OnPlayerComboIncremented += OnIncrement;
        PlayerFsm.OnPlayerComboReset += OnReset;
    }

    private void OnDisable()
    {
        PlayerFsm.OnPlayerComboIncremented -= OnIncrement;
        PlayerFsm.OnPlayerComboReset -= OnReset;
    }
}
