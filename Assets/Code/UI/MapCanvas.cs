using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapCanvas : MonoBehaviour
{

    public static MapCanvas Singleton;
    private PlayerInput _playerInput;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _mainCanvasGroup;
    private CanvasGroup _useConfirmationCanvasGroup;
    private Image _closeImage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Singleton = this;
        _playerInput = GetComponent<PlayerInput>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _mainCanvasGroup = transform.Find("MainView").GetComponent<CanvasGroup>();
        _useConfirmationCanvasGroup = transform.Find("UseConfirmationView").GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0;
        _mainCanvasGroup.alpha = 0;
        _useConfirmationCanvasGroup.alpha = 0;
        
        _closeImage = _mainCanvasGroup.transform.Find("CloseInput").Find("Image").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
