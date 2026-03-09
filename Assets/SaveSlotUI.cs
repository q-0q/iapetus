using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public int saveId = 0;

    public Image _Image;
    public TextMeshProUGUI _idTmp;
    public TextMeshProUGUI _newGameTmp;
    public TextMeshProUGUI _timeTmp;
    public TextMeshProUGUI _bellsTmp;
    public TextMeshProUGUI _lemonsTmp;
    public TextMeshProUGUI _trialsTmp;
    public GameObject _completion;

    private void Awake()
    {
        _idTmp.text = "0" + (saveId + 1);

        var saveData = SaveSystem.LoadSaveDataFromDisk(saveId);
        if (saveData == null)
        {
            _timeTmp.gameObject.SetActive(false);
            _completion.SetActive(false);
            _Image.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _newGameTmp.gameObject.SetActive(false);
            _timeTmp.text = saveData.playTime.ToString();
            _bellsTmp.text = saveData.bells.Count.ToString();
            _lemonsTmp.text = saveData.lemonCollections.Count.ToString();
            _trialsTmp.text = saveData.trialCompletions.Count(e => e.time < e.goldTime).ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
