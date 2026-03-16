using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public int saveId = 0;

    public Image ScreenCaptureImage;
    public TextMeshProUGUI _idTmp;
    public TextMeshProUGUI _newGameTmp;
    public TextMeshProUGUI _timeTmp;
    public TextMeshProUGUI _bellsTmp;
    public TextMeshProUGUI _lemonsTmp;
    public TextMeshProUGUI _trialsTmp;
    public GameObject _completion;
    public GameObject image;

    private void Awake()
    {
        _idTmp.text = "0" + (saveId + 1);

        var saveData = SaveSystem.LoadSaveDataFromId(saveId);
        if (saveData == null)
        {
            _timeTmp.gameObject.SetActive(false);
            _completion.SetActive(false);
            image.SetActive(false);
            ScreenCaptureImage.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            _newGameTmp.gameObject.SetActive(false);
            LoadSaveImage(SaveSystem.GetImagePathFromSaveId(saveId));
            _timeTmp.text = GetPlaytimeString(saveData.playTime);
            _bellsTmp.text = saveData.bells.Count.ToString();
            _lemonsTmp.text = saveData.lemonCollections.Count.ToString();
            _trialsTmp.text = saveData.trialCompletions.Count(e => e.time < e.goldTime).ToString();
        }
    }

    private static string GetPlaytimeString(float seconds)
    {
        int hours = Mathf.FloorToInt(seconds / 3600f);
        int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);

        return $"{hours}h {minutes}m";
    }

    private void LoadSaveImage(string path)
    {
        
        if (File.Exists(path))
        {
            byte[] fileData = File.ReadAllBytes(path);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); // Automatically resizes texture

            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f)
            );

            ScreenCaptureImage.sprite = sprite;
            ScreenCaptureImage.preserveAspect = true;
        }
        else
        {
            Debug.LogWarning("Image not found: " + path);
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
