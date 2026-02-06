using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboCanvas : MonoBehaviour
{
    private TextMeshProUGUI _tmp;
    
    // Start is called before the first frame update
    void Start()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var comboLength = PlayerFsm.Singleton.GetComboLength();
        _tmp.text = comboLength.ToString();
        _tmp.color = comboLength >= PlayerFsm.MaxComboLength ? Color.magenta : Color.white;
    }
}
