using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLightParentScalar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out Light _light);
        
    }
    
}
