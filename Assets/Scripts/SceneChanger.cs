using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        Time.timeScale = 1.0f;
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName); 
        }
        else
        {
            Debug.LogError("Scene name is empty or invalid!");
        }
    }

    public void doExitGame()
    {
        Application.Quit();
    }
}

