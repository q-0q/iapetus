
using TMPro;
using UnityEngine;

public class FpsTracker
    : MonoBehaviour
{
    private float fps = 60f;
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float newFPS = 1.0f / Time.deltaTime;
        fps = Mathf.Lerp(fps, newFPS, 0.005f);
        _tmp.text = "FPS: " + ((int)fps);
    }
}
