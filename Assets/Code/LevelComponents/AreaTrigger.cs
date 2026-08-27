using System;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public string id;
    public static event Action<string> OnAreaTrigger;

    private void Start()
    {
        if (SaveSystem.GetArea(id))
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (SaveSystem.GetArea(id))
        {
            gameObject.SetActive(false);
            return;
        }
        
        gameObject.SetActive(false);
        SaveSystem.WriteArea(id);
        OnAreaTrigger?.Invoke(id);
    }
}
