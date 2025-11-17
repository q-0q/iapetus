using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Singleton;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    public void LoadScene(string scene)
    {
        StartCoroutine(LoadYourAsyncScene(scene));
    }
    
    IEnumerator LoadYourAsyncScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        _canvasGroup.alpha = 1f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        _canvasGroup.alpha = 0f;
    }
}
