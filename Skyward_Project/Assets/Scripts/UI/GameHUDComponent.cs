using System;
using Skyward.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameHUDComponent : MonoBehaviour, ISkywardComponent
{
    public GameObject pauseScreen;
    public GameObject playerUIScreen;
    public GameObject quitScreen;


    private void Start()
    {
        OpenPlayerUIScreen();
    }

    public void OpenPauseScreen()
    {
        pauseScreen.SetActive(true);
        playerUIScreen.SetActive(true);
        quitScreen.SetActive(false);
    }

    public void OpenPlayerUIScreen()
    {
        pauseScreen.SetActive(false);
        playerUIScreen.SetActive(true);
        quitScreen.SetActive(false);
    }

    public void OpenQuitScreen()
    {
        pauseScreen.SetActive(false);
        playerUIScreen.SetActive(false);
        quitScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
