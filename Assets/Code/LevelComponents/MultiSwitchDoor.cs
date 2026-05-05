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
    private CinemachineVirtualCamera _lightVirtualCamera;
    private CinemachineVirtualCamera _openVirtualCamera;
    private Collider _openTrigger;
    private PowerConnector _powerConnector;

    public string persistentEvent;

    private Transform _cameraFollow;
    private Transform _cameraStart;
    private Transform _cameraEnd;

    private void Awake()
    {
        _cameraFollow = transform.Find("Camera").Find("CameraFollow");
        _cameraStart = transform.Find("Camera").Find("CameraFollowStart");
        _cameraEnd = transform.Find("Camera").Find("CameraFollowEnd");
        _powerConnector = GetComponentInChildren<PowerConnector>();
        
        TryGetComponent(out _openTrigger);
        _openTrigger.enabled = false;
        
        _lightVirtualCamera = transform.Find("Camera").Find("MultiSwitchDoorLightVirtualCamera").GetComponentInChildren<CinemachineVirtualCamera>();
        _openVirtualCamera = transform.Find("Camera").Find("MultiSwitchDoorOpenVirtualCamera").GetComponentInChildren<CinemachineVirtualCamera>();
        
        var lightPrefab = Resources.Load("Prefab/MultiSwitchDoorLight") as GameObject;
        var lightHolder = transform.Find("DoorLights").Find("Lights");
        _lightDictionary = new Dictionary<OnetimeSwitchFsm, GameObject>();
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
        }

        if (SaveSystem.GetPersistentEventCompleted(persistentEvent))
        {
            _powerConnector.Source = true;
            _openTrigger.enabled = false;
            GetComponentInChildren<MovingPlatform>().JumpToEnd();
        } 
        else if (IsAllSwitchesEnabled())
        {
            _openTrigger.enabled = true;
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
        if (!IsAllSwitchesEnabled()) return;
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
            FMODUnity.RuntimeManager.PlayOneShotAttached(FMODUnity.RuntimeManager.PathToEventReference("event:/MultiSwitchDoorLight"), _lightDictionary[switchFsm].gameObject);
            
            yield return null;
        }
        
        IEnumerator CameraCoroutine()
        {
            CutsceneManager.Singleton.SetPseudoCutsceneActive(true);
            yield return new WaitForSeconds(0.75f);
            yield return new WaitForSeconds(0.65f);
            _cameraFollow.position = _cameraStart.position;
            float t = 0;
            float duration = 1.25f;
            _lightVirtualCamera.Priority = 20;
            yield return new WaitForSeconds(0.5f);
            while (t < duration)
            {
                _cameraFollow.position = Vector3.Lerp(_cameraStart.position, _cameraEnd.position, Util.SmoothLerp01(t/duration));
                yield return null;
                t += Time.deltaTime;
            }
            
            yield return new WaitForSeconds(1.2f);

            
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _lightVirtualCamera.Priority = -10;
            
            if (IsAllSwitchesEnabled())
            {
                _openTrigger.enabled = true;
            }
            
            yield return null;
        }
    }

    private bool IsAllSwitchesEnabled()
    {
        foreach (var switchFsm in SwitchFsms)
        {
            if (!SaveSystem.GetPersistentEventCompleted(switchFsm.persistentEvent)) return false;
        }

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        SaveSystem.WritePersistentEvent(persistentEvent);
        _openTrigger.enabled = false;

        StartCoroutine(CameraCoroutine());
        
        IEnumerator CameraCoroutine()
        {
            CutsceneManager.Singleton.SetPseudoCutsceneActive();
            _openVirtualCamera.Priority = 20;
            yield return new WaitForSeconds(0.75f);
            _powerConnector.Source = true;
            yield return new WaitForSeconds(2.5f);
            CutsceneManager.Singleton.ClearPseudoCutsceneActive();
            _openVirtualCamera.Priority = -10;
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
