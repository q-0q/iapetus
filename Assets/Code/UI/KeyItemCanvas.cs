using System;
using System.Collections;
using Code.Misc;
using TMPro;
using UnityEngine;

public class KeyItemCanvas : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _itemText;

    private void OnItemCollected(string displayName)
    {
        _itemText.text = displayName;
        StartCoroutine(CanvasCoroutine());

        IEnumerator CanvasCoroutine()
        {
            var t = 0f;
            var d = 0.5f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _canvasGroup.alpha = w;
                t += Time.deltaTime;
                yield return null;
            }

            t = 0;
            d = 3.5f;
            while (t < d)
            {
                if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.InventorySlowdown))
                {
                    _canvasGroup.alpha = 0;
                    yield break;
                }
                t += Time.deltaTime;
                yield return null;
            }

            t = 0;
            d = 1f;
            while (t < d)
            {
                var w = Util.SmoothLerp01(t / d);
                _canvasGroup.alpha = 1f - w;
                t += Time.deltaTime;
                yield return null;
            }

            _canvasGroup.alpha = 0f;

        }
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _itemText = transform.Find("ItemText").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        KeyItem.OnKeyItemCollected += OnItemCollected;
        CultistIncenseFsm.OnIncenseGiven += OnItemCollected;
        PlayerFsm.OnItemCollected += OnItemCollected;
    }

    private void OnDisable()
    {
        KeyItem.OnKeyItemCollected -= OnItemCollected;
        CultistIncenseFsm.OnIncenseGiven -= OnItemCollected;
        PlayerFsm.OnItemCollected -= OnItemCollected;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
