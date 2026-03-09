using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private GameObject _main;
    private GameObject _levelSelect;
    private GameObject _settings;
    private GameObject _credits;
    private GameObject _current;
    private GameObject _back;

    private string _scene;
    // Start is called before the first frame update
    void Start()
    {
        
        FMODSceneManager.Singleton.StopAll();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        _main = transform.Find("Main").gameObject;
        _back = transform.Find("Back").gameObject;
        _levelSelect = transform.Find("LevelSelect").gameObject;
        // _settings = transform.Find("SettingsMenu").gameObject;
        // _credits = transform.Find("Credits").gameObject;
        
        // _settings.SetActive(false);
        // _credits.SetActive(false);

        var saveData = SaveSystem.LoadCachedSaveData();
        _scene = saveData.scene;
        _levelSelect.transform.Find("Buttons").Find("Demo").GetComponentInChildren<TextMeshProUGUI>().text =
            _scene == "" ? "New Game" : "Continue";
        
        
        _levelSelect.SetActive(false);
        _back.SetActive(false);
    }

    public void OnPlayClicked()
    {
        _main.SetActive(false);
        _back.SetActive(true);
        _levelSelect.SetActive(true);
        _levelSelect.transform.Find("Buttons").Find("Demo").GetComponent<Button>().Select();
    }
    
    public void OnSettingsClicked()
    {
        _main.SetActive(false);
        _settings.SetActive(true);
    }
    
    public void OnCreditsClicked()
    {
        _main.SetActive(false);
        _credits.SetActive(true);
        _back.SetActive(true);
        _back.GetComponent<Button>().Select();
    }
    
    private void OnEnable()
    {
        SettingsMenu.OnSettingsMenuClosed += OnSettingsClosed;
    }
    
    private void OnDisable()
    {
        SettingsMenu.OnSettingsMenuClosed -= OnSettingsClosed;
    }

    private void OnSettingsClosed()
    {
        OnBackClicked();
    }

    public void OnBackClicked()
    {
        _back.SetActive(false);
        _levelSelect.SetActive(false);
        // _credits.SetActive(false);
        _main.SetActive(true);
        _main.transform.Find("Buttons").Find("Play").GetComponent<Button>().Select();
    }

    public void OnLevelSelected(string scene)
    {
        SceneLoader.Singleton.LoadScene(_scene == "" ? "C1-Brazier" : _scene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
