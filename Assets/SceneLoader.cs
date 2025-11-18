using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton;
    private CanvasGroup _canvasGroup;
    private RectTransform _rawImageRectTransform;

    private const float InDuration = 0.3f;
    private const float OutDuration = 1f;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _rawImageRectTransform = GetComponentInChildren<RawImage>().GetComponent<RectTransform>();
        StartCoroutine(OnStart());
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadYourAsyncScene(scene));
    }

    IEnumerator OnStart()
    {
        Vector2 startPos = _rawImageRectTransform.anchoredPosition;
        float width = _rawImageRectTransform.rect.width;
        Vector2 endPos = startPos - new Vector2(width, 0);

        float elapsed = 0f;

        while (elapsed < InDuration)
        {
            elapsed += Time.deltaTime;
            float t = SmoothLerp01(elapsed / InDuration);
            _rawImageRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        _rawImageRectTransform.anchoredPosition = endPos;
    }
    
    IEnumerator LoadYourAsyncScene(string scene)
    {
        float width = _rawImageRectTransform.rect.width;
        Vector2 startPos = new Vector2(width, 0);
        Vector2 endPos = startPos - new Vector2(width, 0);
        float elapsed = 0f;
        while (elapsed < InDuration)
        {
            elapsed += Time.deltaTime;
            float t = SmoothLerp01(elapsed / InDuration);
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
    }
    
    public static float SmoothLerp01(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

}
