using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTerrain : MonoBehaviour
{
    private Collider _collider;
    private bool _disabling;
    
    // Start is called before the first frame update
    void Start()
    {
        _disabling = false;
        TryGetComponent(out _collider);
    }

    // Update is called once per frame
    void Update()
    {
        IEnumerator DisableAfterDelay()
        {
            if (_disabling) yield break;
            _disabling = true;
            yield return new WaitForSeconds(0.35f);
            _collider.enabled = false;
            _disabling = false;
        }

        if (_collider.enabled && !PlayerFsm.Singleton.GetIsSurging())
            StartCoroutine(DisableAfterDelay());

        if (!_collider.enabled && PlayerFsm.Singleton.GetIsSurging())
            _collider.enabled = true;

    }
}
