using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool isPaused;

    [Header("MainMenu")]
    public string _mainMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            ActivateMenu();
        }

        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0; // Freezes time
        AudioListener.pause = true;  // Sets Audio from player to pause
        pauseMenuUI.SetActive(true);
    }

    public void DeactivateMenu() // public so resume button can acces the function
    {
        Time.timeScale = 1; // Resets Time to normal
        AudioListener.pause = false; //Activates Audio
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }

    // On confirm load MainMenu Scene
    public void LoadMainMenuYes()
    {
        SceneManager.LoadScene(_mainMenu);
    }
}
