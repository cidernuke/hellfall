using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private bool isPaused;

    [SerializeField] private AudioSource musicAudioSource;

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
        Time.timeScale = 0; // Freeze time
        pauseMenuUI.SetActive(true);

        // Pause all audio sources except the music
        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio != musicAudioSource)
            {
                audio.Pause();
            }
        }
    }

    public void DeactivateMenu() // public so resume button can acces the function
    {
        Time.timeScale = 1; // Resume time
        pauseMenuUI.SetActive(false);
        isPaused = false;

        // Resume all audio sources except the music
        foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
        {
            if (audio != musicAudioSource)
            {
                audio.UnPause();
            }
        }
    }

    // On confirm load MainMenu Scene
    public void LoadMainMenuYes()
    {
        SceneManager.LoadScene(_mainMenu);
    }
}

