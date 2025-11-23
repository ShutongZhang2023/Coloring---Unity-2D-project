using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    [SerializeField] private string uiSceneName = "UIScene";
    [SerializeField] private string mapSceneName = "MapScene";
    [SerializeField] private string otherSceneName = "OtherScene";

    void Start()
    {
        LoadIfNotLoaded(uiSceneName);
        LoadIfNotLoaded(mapSceneName);
        LoadIfNotLoaded(otherSceneName);
    }

    void LoadIfNotLoaded(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}
