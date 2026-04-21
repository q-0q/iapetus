using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Util = Code.Misc.Util;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton;
    private CanvasGroup _canvasGroup;
    private RectTransform _rawImageRectTransform;

    private const float InDuration = 0.5f;
    private const float HoldDuration = 0.2f;
    private const float OutDuration = 0.3f;

    private bool _mutex;

    private void Awake()
    {
        Singleton = this;
        _mutex = false;
    }

    private void Start()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _rawImageRectTransform = GetComponentInChildren<RawImage>().GetComponent<RectTransform>();
        StartCoroutine(OnStart());
    }

    public void LoadScene(string scene)
    {
        // FMODUnity.RuntimeManager.GetBus("Bus:/").stopAllEvents(STOP_MODE.IMMEDIATE);
        StartCoroutine(LoadYourAsyncScene(scene));
    }

    IEnumerator OnStart()
    {
        _rawImageRectTransform.anchorMin = Vector2.zero;
        _rawImageRectTransform.anchorMax = Vector2.one;
        _rawImageRectTransform.offsetMin = Vector2.zero;
        _rawImageRectTransform.offsetMax = Vector2.zero;
        Vector2 startPos = _rawImageRectTransform.anchoredPosition;
        float width = _rawImageRectTransform.rect.width;
        Vector2 endPos = startPos - new Vector2(width, 0);

        float elapsed = 0f;
        while (elapsed < HoldDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < OutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Util.SmoothLerp01(elapsed / OutDuration);
            _rawImageRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        _rawImageRectTransform.anchoredPosition = endPos;
    }
    
    IEnumerator LoadYourAsyncScene(string scene)
    {
        if (_mutex) yield break;
        _mutex = true;
        float width = _rawImageRectTransform.rect.width;
        Vector2 startPos = new Vector2(width, 0);
        Vector2 endPos = startPos - new Vector2(width, 0);
        float elapsed = 0f;
        while (elapsed < InDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Util.SmoothLerp01(elapsed / InDuration);
            _rawImageRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        _rawImageRectTransform.anchoredPosition = endPos;
        
        _canvasGroup.alpha = 1f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        _canvasGroup.alpha = 0f;
        _mutex = false;
    }

}
