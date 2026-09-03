using TMPro;
using UnityEngine;

public class CPLTracker : MonoBehaviour
{
    private TextMeshProUGUI _tmp;

    private void Awake()
    {
        _tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        _tmp.text = "CPLs: " + Shader.GetGlobalInt("_CustomPointLightCount");
    }

}
