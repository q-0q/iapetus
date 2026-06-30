using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionCanvas : MonoBehaviour
{
    public static InteractionCanvas Singleton;
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _tmp;
    public Image Image;
    private bool psuedo;
    private string psuedoText;
    private RectTransform rectTransform;

    private void Awake()
    {
        Singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = _tmp.transform.parent.GetComponent<RectTransform>();
        _canvasGroup.alpha = 0;
        psuedo = false;
    }

    // Update is called once per frame
    void Update()
    {
        Image.sprite = InputTypeManager.Singleton.GetSpriteForAction("Interact");

        if (PhotoManager.Singleton.IsActive())
        {
            _canvasGroup.alpha = 0f;
            return;
        }

        if (psuedo && !GameMenu.Singleton.IsMenuOpen() && !AcquisitionCanvas.Singleton.isOpen)
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, Time.unscaledDeltaTime * 15f);
            _tmp.text = psuedoText;
            return;
        }
        
        var interactable = PlayerFsm.Singleton.currentPotentialInteractable;
        if (interactable is not null && !GameMenu.Singleton.IsMenuOpen() && DialogueCanvas.Singleton.TimeSinceDialogueClosed > 0.5f && !AcquisitionCanvas.Singleton.isOpen)
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, Time.unscaledDeltaTime * 10f);
            _tmp.text = interactable.text;
            _tmp.CalculateLayoutInputHorizontal();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        else
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, Time.unscaledDeltaTime * 40f);
        }
    }

    public void SetPsuedoInteractable(string text)
    {
        psuedo = true;
        psuedoText = text;
    }

    public void ClearPsuedoInteractable()
    {
        psuedo = false;
    }
}
