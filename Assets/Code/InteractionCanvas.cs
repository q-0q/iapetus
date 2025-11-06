using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionCanvas : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _tmp;
    
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
        var interactable = PlayerFsm.Singleton.currentPotentialInteractable;
        if (interactable is not null)
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, Time.deltaTime * 15f);
            _tmp.text = "[E] " + interactable.text;
        }
        else
        {
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, Time.deltaTime * 40f);
        }
    }
}
