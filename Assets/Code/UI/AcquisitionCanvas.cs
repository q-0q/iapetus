using System;
using System.Collections;
using Code.Misc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcquisitionCanvas : MonoBehaviour
{
    public static event Action OnAcquisitionCanvasOpened;
    public static event Action OnAcquisitionCanvasClosed;

    public CanvasGroup _backgroundCanvasGroup;
    public CanvasGroup _upperCanvasGroup;
    public CanvasGroup _middleCanvasGroup;
    public CanvasGroup _lowerCanvasGroup;

    public TextMeshProUGUI _upperSubtext;
    public TextMeshProUGUI _upperText;
    public Image _inputImage;
    public TextMeshProUGUI _inputClauseText;
    public TextMeshProUGUI _descriptionText;
    
    
    private void OnEnable()
    {
        PlayerFsm.OnTrickAcquired += OnTrickAcquired;
    }

    private void OnDisable()
    {
        PlayerFsm.OnTrickAcquired -= OnTrickAcquired;
    }

    private void Awake()
    {
        _backgroundCanvasGroup.alpha = 0;
        _upperCanvasGroup.alpha = 0;
        _middleCanvasGroup.alpha = 0f;
        _lowerCanvasGroup.alpha = 0;
    }

    private void OnTrickAcquired(string trick)
    {
        var data = TrickRegistry.TrickRegistrations[trick];
        StartCoroutine(Coroutine("Lotus Form learned", data.displayName, data.useInput, data.useClause, data.description));
    }

    IEnumerator Coroutine(string upperSubtext, string upperText, string input, string inputClauseText, string description)
    {

        _upperSubtext.text = upperSubtext;
        _upperText.text = upperText;
        // _inputImage.sprite = InputTypeManager.Singleton.GetSpriteForAction(input);
        _inputClauseText.text = inputClauseText;
        _descriptionText.text = description;

        var t = 0f;
        var d = 1.5f;

        while (t < d)
        {
            var w = Util.SmoothLerp01(t / d );
            _backgroundCanvasGroup.alpha = Mathf.Lerp(_backgroundCanvasGroup.alpha, 1f, Time.deltaTime * 4f);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        
        t = 0f;
        d = 1f;

        while (t < d)
        {
            var w = t / d;
            _upperCanvasGroup.alpha = w;
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        
        t = 0f;
        d = 1f;

        while (t < d)
        {
            var w = t / d;
            _middleCanvasGroup.alpha = w;
            t += Time.deltaTime;
            yield return null;
        }
        
        t = 0f;
        d = 1f;

        while (t < d)
        {
            var w = t / d;
            _lowerCanvasGroup.alpha = w;
            t += Time.deltaTime;
            yield return null;
        }

        _backgroundCanvasGroup.alpha = 1f;
        _upperCanvasGroup.alpha = 1f;
        _middleCanvasGroup.alpha = 1f;
        _lowerCanvasGroup.alpha = 1f;
        OnAcquisitionCanvasOpened?.Invoke();

        yield return new WaitForSeconds(3f);
        
        t = 0f;
        d = 1.5f;

        while (t < d)
        {
            var w = 1f - (t / d);
            _backgroundCanvasGroup.alpha = w;
            _upperCanvasGroup.alpha = w;
            _middleCanvasGroup.alpha = w;
            _lowerCanvasGroup.alpha = w;
            t += Time.deltaTime;
            yield return null;
        }
        
        _backgroundCanvasGroup.alpha = 0;
        _upperCanvasGroup.alpha = 0;
        _middleCanvasGroup.alpha = 0f;
        _lowerCanvasGroup.alpha = 0;
        OnAcquisitionCanvasClosed?.Invoke();
        
        yield return null;
    }
}
