using UnityEngine;
using UnityEngine.InputSystem;

public class UIScript : MonoBehaviour
{
    public GameObject PauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseMenu?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePause()
    {
        if (!PauseMenu.activeSelf)
        {
            Pause();
        } else
        {
            Resume();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        PauseMenu?.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        PauseMenu?.SetActive(false);
    }
}
