using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavapheCrossTrigger : MonoBehaviour
{

    public static event Action SavapheCrossTriggerOnTriggerEnter;
    private void OnTriggerEnter(Collider other)
    {
        SavapheCrossTriggerOnTriggerEnter?.Invoke();
    }
}
