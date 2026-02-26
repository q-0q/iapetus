using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Code.Misc;
using UnityEngine;

public class MultiSwitchDoor : MonoBehaviour
{

    public List<OnetimeSwitchFsm> SwitchFsms;
    private Dictionary<OnetimeSwitchFsm, GameObject> _lightDictionary;
    private CinemachineVirtualCamera _virtualCamera;

    public string persistentEventPrefix;

    private Transform _cameraFollow;
    private Transform _cameraStart;
    private Transform _cameraEnd;

    private void Awake()
    {
        _cameraFollow = transform.Find("Camera").Find("CameraFollow");
        _cameraStart = transform.Find("Camera").Find("CameraFollowStart");
        _cameraEnd = transform.Find("Camera").Find("CameraFollowEnd");
        
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        var lightPrefab = Resources.Load("Prefab/MultiSwitchDoorLight") as GameObject;
        var lightHolder = transform.Find("DoorLights").Find("Lights");
        _lightDictionary = new Dictionary<OnetimeSwitchFsm, GameObject>();
        var isAllSwitchesPersistentEventsActive = true;
        for (int i = 0; i < SwitchFsms.Count; i++)
        {
            var position = Vector3.down * i * 2f;
            var obj = Instantiate(lightPrefab, lightHolder);
            obj.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
            _lightDictionary.Add(SwitchFsms[i], obj);
            if (SaveSystem.GetPersistentEventCompleted(SwitchFsms[i].persistentEvent))
            {
                obj.GetComponentInChildren<Renderer>().material.SetFloat("_Weight", 1f);
            }
            else
            {
                isAllSwitchesPersistentEventsActive = false;
            }
        } 
    }

    private void OnEnable()
    {
        OnetimeSwitchFsm.OnOnetimeSwitchFsmTurnedOn += OnSwitch;
    }

    private void OnDisable()
    {
        OnetimeSwitchFsm.OnOnetimeSwitchFsmTurnedOn -= OnSwitch;
    }

    private void OnSwitch(OnetimeSwitchFsm switchFsm)
    {

        StartCoroutine(MaterialCoroutine());
        StartCoroutine(CameraCoroutine());

        IEnumerator MaterialCoroutine()
        {
            yield return new WaitForSeconds(2f);
            float t = 0;
            float duration = 1.25f;
            var material = _lightDictionary[switchFsm].GetComponentInChildren<Renderer>().material;
            while (t < duration)
            {
                material.SetFloat("_Weight", t / duration);
                yield return null;
                t += Time.deltaTime;
            }
            
            Util.InvokeSphereEffect(_lightDictionary[switchFsm].transform.position + Vector3.down, Vector3.one * 2f, 1.25f, 1f, 0f);
            
            yield return null;
        }
        
        IEnumerator CameraCoroutine()
        {
            yield return new WaitForSeconds(1f);
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
            yield return new WaitForSeconds(0.75f);
            _cameraFollow.position = _cameraStart.position;
            float t = 0;
            float duration = 1.25f;
            _virtualCamera.Priority = 20;
            yield return new WaitForSeconds(0.5f);
            while (t < duration)
            {
                _cameraFollow.position = Vector3.Lerp(_cameraStart.position, _cameraEnd.position, Util.SmoothLerp01(t/duration));
                yield return null;
                t += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(0.5f);

            
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _virtualCamera.Priority = -10;
            yield return null;
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
