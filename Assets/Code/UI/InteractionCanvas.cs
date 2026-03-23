using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionCanvas : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _tmp;
    public Image Image;
    
    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
        _canvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Image.sprite = InputTypeManager.Singleton.GetSpriteForAction("Interact");
        var interactable = PlayerFsm.Singleton.currentPotentialInteractable;
        if (interactable is not null && !GameMenu.Singleton.IsMenuOpen())
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, Time.unscaledDeltaTime * 15f);
            _tmp.text = interactable.text;
        }
        else
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, Time.unscaledDeltaTime * 40f);
        }
    }
}
