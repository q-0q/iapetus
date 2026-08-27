using System;
using System.Collections;
using FMOD.Studio;
using TMPro;
using UnityEngine;

public class AreaTitleCanvas : MonoBehaviour
{


    public static AreaTitleCanvas Singleton;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI subText;
    public TextMeshProUGUI coordsText;
    public TextMeshProUGUI elevationText;
    private CanvasGroup _canvasGroup;
    public RectTransform border;

    private void Awake()
    {
        Singleton = this;
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    private void OnEnable()
    {
        AreaTrigger.OnAreaTrigger += OnAreaTrigger;
    }

    private void OnDisable()
    {
        AreaTrigger.OnAreaTrigger -= OnAreaTrigger;
    }

    private void OnAreaTrigger(string id)
    {
        _canvasGroup.alpha = 0;

        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            var t = 0f;
            var d = 2.5f;
            border.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);

            var data = GlyphController.AreaRegistry[id];
            
            if (data == null) yield break;

            subText.text = data.subtitle;
            mainText.text = data.title;
            coordsText.text = data.coords;
            elevationText.text = data.elevation;

            while (t < d)
            {
                _canvasGroup.alpha = Mathf.Clamp01(Mathf.InverseLerp(0, 1.5f, t));
                border.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(0, 400, Code.Misc.Util.SmoothLerp01(t/d)));
                t += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(3f);
            
            
            t = 0f;
            d = 1.5f;
            
            while (t < d)
            {
                _canvasGroup.alpha = 1f - Mathf.Clamp01(t / d);
                t += Time.deltaTime;
                yield return null;
            }
            
            _canvasGroup.alpha = 0;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
