using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
        _main = transform.Find("Main").gameObject;
        _back = transform.Find("Back").gameObject;
        _levelSelect = transform.Find("LevelSelect").gameObject;
        _settings = transform.Find("SettingsMenu").gameObject;
        // _credits = transform.Find("Credits").gameObject;
        
        _levelSelect.SetActive(false);
        _back.SetActive(false);
    }

    public void OnPlayClicked()
    {
        _main.SetActive(false);
        _back.SetActive(true);
        _levelSelect.SetActive(true);
        _levelSelect.transform.Find("Buttons").Find("Tutorial").GetComponent<Button>().Select();
    }
    
    public void OnSettingsClicked()
    {
        _main.SetActive(false);
        _settings.SetActive(true);
    }
    
    public void OnCreditsClicked()
    {
        
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
        _main.SetActive(true);
        _main.transform.Find("Buttons").Find("Play").GetComponent<Button>().Select();
    }

    public void OnLevelSelected(string scene)
    {
        SceneLoader.Singleton.LoadScene(scene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
