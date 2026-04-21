using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutsceneTrigger : MonoBehaviour
{
    public string id = "";
    public static event Action<string> OnEnter;

    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(id);
    }
}
