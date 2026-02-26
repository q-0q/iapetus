using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSwitchDoor : MonoBehaviour
{

    public List<SwitchFsm> SwitchFsms;

    private void Awake()
    {
        var lightPrefab = Resources.Load("Prefab/MultiSwitchDoorLight") as GameObject;
        var lightHolder = transform.Find("DoorLights").Find("Lights");
        for (int i = 0; i < SwitchFsms.Count; i++)
        {
            var position = Vector3.down * i * 2f;
            var obj = Instantiate(lightPrefab, lightHolder);
            obj.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
            
        } 
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
