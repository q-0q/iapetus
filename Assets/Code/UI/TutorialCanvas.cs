using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialCanvas : MonoBehaviour
{
    
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _tmpText;
    public static TutorialCanvas Singleton;
    private bool _open = false;
    private int _currentTextIndex = 0;

    private void Awake()
    {
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tmpText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        _canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_open && !GameMenu.Singleton.IsMenuOpen())
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, Time.unscaledDeltaTime * 10f);
        }
        else
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, Time.unscaledDeltaTime * 5f);
        }
    }

    public void HideTutorialText()
    {
        _open = false;
    }

    public void ShowTutorialText(string text)
    {
        _open = true;
        _tmpText.text = text;
    }
    
}
