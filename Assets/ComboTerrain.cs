using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTerrain : MonoBehaviour
{
    private Collider _collider;
    
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out _collider);
    }

    // Update is called once per frame
    void Update()
    {
        _collider.enabled = PlayerFsm.Singleton.GetComboLength() >= PlayerFsm.MaxComboLength;
    }
}
