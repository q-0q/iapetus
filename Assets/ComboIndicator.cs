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
        var radius = 0.45f;
        Vector3 offset = new Vector3(2f, 1f, 0);
        for (int i = 0; i < PlayerFsm.MaxComboLength; i++)
        {
            var position = Quaternion.Euler(0, 0, 360f * i / PlayerFsm.MaxComboLength) * (transform.up * radius);
            position += offset;
            var obj = Instantiate(prefab, transform);
            obj.transform.localPosition = position;
            print("instantiated " + obj.name);
        }
        transform.SetParent(null);
        OnReset();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = PlayerFsm.Singleton.transform.position;
    }

    private void OnIncrement(int length)
    {
        IEnumerator Kill()
        {
            yield return new WaitForSeconds(1.0f);
            OnReset();
        }
        
        if (length == PlayerFsm.MaxComboLength) StartCoroutine(Kill());
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
