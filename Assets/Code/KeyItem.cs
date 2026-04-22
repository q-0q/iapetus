using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using Util = Code.Misc.Util;

public class KeyItemRegistration
{
    public string displayName;
    public GameObject MeshGameObject;
    public Sprite Sprite;
    public string description;
    
    public Func<string> GetUseDescription = () => "It doesn't seem to have much use.";
    public Func<bool> GetCanUse = () => false;
    public Action onUse = null;
    public Func<string> GetUseConfirmation = () => "Use this item?";
}

public static class KeyItemRegistry
{
    public static readonly Dictionary<string, KeyItemRegistration> KeyItemRegistrations;

    static KeyItemRegistry()
    {
        KeyItemRegistrations = new Dictionary<string, KeyItemRegistration>();
        
        KeyItemRegistrations.Add("ErhuFragment1", new KeyItemRegistration()
        {
            displayName = "Erhu neck",
            description = "The stick-like neck of a delicate instrument. It's broken, but may be repaired.",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            Sprite = null,
        });
        
        KeyItemRegistrations.Add("ErhuFragment2", new KeyItemRegistration()
        {
            displayName = "Erhu tuner",
            description = "A series of intricate tuning pegs. They're broken, but may be repaired.",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            Sprite = null,
        });
        
        KeyItemRegistrations.Add("ErhuFragment3", new KeyItemRegistration()
        {
            displayName = "Erhu body",
            description = "The resonator body of a musical instrument. It's broken, but may be repaired.",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            Sprite = null,
        });
        
        KeyItemRegistrations.Add("IncenseBurner", new KeyItemRegistration()
        {
            displayName = "Incense burner",
            description = "An ornately constructed censer etched with fine runes. A voice seems to whisper within...",
            MeshGameObject = Resources.Load("Prefab/KeyItems/UrnFragment") as GameObject,
            Sprite = null,
            GetUseDescription = () =>
            {
                var count = SaveSystem.GetIncenseAmount();
                var nearby = PlayerFsm.Singleton.GetNearbyCultTrial(out var fsm);
                if (count == 0 || nearby) return count.ToString() + " cones of incense left.";
                return count.ToString() + " cones of incense left, but it wouldn't do anything right now.";
            },
            GetUseConfirmation = () => "Burn a cone of incense?"
        });
        
    }
}


public class KeyItem : MonoBehaviour
{

    public string Id;
    private Interactable _interactable;
    private Transform _meshTransform;
    private bool collected = false;
    private ParticleSystem _particleSystem;

    public static event Action<KeyItemRegistration> OnKeyItemCollected;
    private const string GenericCollectionPersistentEvent = "item-collected";

    private void Awake()
    {
        _interactable = GetComponentInChildren<Interactable>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        
        if (SaveSystem.GetPersistentEventCompleted(Id)) Destroy(gameObject);
    }

    private void OnEnable()
    {
        _interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        _interactable.OnInteracted -= OnInteracted;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var data = KeyItemRegistry.KeyItemRegistrations[Id];
        _meshTransform = Instantiate(data.MeshGameObject, transform).transform;
    }

    private void OnInteracted()
    {
        
        if (collected) return;
        collected = true;
        _interactable.SetEnabled(false);
        
        StartCoroutine(PositionCoroutine());
        StartCoroutine(RotationCoroutine());
        StartCoroutine(ScaleCoroutine());
        
        PlayerFsm.Singleton.Machine.Jump(PlayerFsm.PlayerFsmState.KeyItemCollect);
        
        IEnumerator PositionCoroutine()
        {
            var t = 0f;
            var d = 0.5f;

            var start = transform.position;
            var end = PlayerFsm.Singleton.transform.position + Vector3.up * 6.5f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                transform.position = Util.LerpWithArc(start, end, w, 1f);
                _meshTransform.localPosition = Vector3.Lerp(_meshTransform.localPosition, Vector3.zero, w);
                _particleSystem.transform.position = _meshTransform.position;
                t += Time.deltaTime;
                yield return null;
            }

            _particleSystem.Stop();
            yield return new WaitForSeconds(0.7f);
            
            t = 0f;
            d = 0.5f;
            
            start = transform.position;
            end = PlayerFsm.Singleton.transform.position + Vector3.up * 2.5f + PlayerFsm.Singleton.transform.forward;
            
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                transform.position = Vector3.Lerp(start, end, w);
                t += Time.deltaTime;
                yield return null;
            }
            
            Collect();
        }
        
        IEnumerator ScaleCoroutine()
        {
            var t = 0f;
            var d = 0.75f;
            yield return new WaitForSeconds(1.3f);
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _meshTransform.localScale = Vector3.Lerp(_meshTransform.localScale, Vector3.zero, w);
                t += Time.deltaTime;
                yield return null;
            }
        }
        
        IEnumerator RotationCoroutine()
        {
            var t = 0f;
            var d = 1.5f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _meshTransform.Rotate(Vector3.up, Time.deltaTime * Mathf.Lerp(1000f, 0f, Mathf.Pow(w, 2f)));
                t += Time.deltaTime;
                yield return null;
            }

        }
    }

    private void Collect()
    {
        var data = KeyItemRegistry.KeyItemRegistrations[Id];
        OnKeyItemCollected?.Invoke(data);
        SaveSystem.WriteItem(Id);
        SaveSystem.WritePersistentEvent(Id);

        if (!SaveSystem.GetPersistentEventCompleted(GenericCollectionPersistentEvent))
        {
            StartCoroutine(Coroutine());
            IEnumerator Coroutine()
            {
                yield return new WaitForSeconds(4f);
                TutorialCanvas.Singleton.ShowTutorialText("Open bag", "Inventory");
            }
            SaveSystem.WritePersistentEvent(GenericCollectionPersistentEvent);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (collected) return;
        _meshTransform.Rotate(Vector3.up, Time.deltaTime * 130f);
        _meshTransform.localPosition = new Vector3(0, (Mathf.Sin(Time.time * 2f) + 1f) * 0.5f,0);
        _particleSystem.transform.position = _meshTransform.position;
    }
}
