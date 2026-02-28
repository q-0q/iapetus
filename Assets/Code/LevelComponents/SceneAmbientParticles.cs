using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAmbientParticles : MonoBehaviour
{
    void Start()
    {
        transform.SetParent(PlayerFsm.Singleton.transform);
        transform.localPosition = Vector3.zero;
    }
    
}
