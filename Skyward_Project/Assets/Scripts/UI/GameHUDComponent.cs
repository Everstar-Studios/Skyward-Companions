using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHUDComponent : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject playerUIScreen;
    public GameObject quitScreen;

    private void Start()
    {
        OpenPlayerUIScreen();
    }

    public void OpenPlayerUIScreen()
    {
        pauseScreen.SetActive(false);
        playerUIScreen.SetActive(true);
        quitScreen.SetActive(false);
    }
    public void OpenPauseScreen()
    {
        pauseScreen.SetActive(true);
        playerUIScreen.SetActive(true);
        quitScreen.SetActive(false);
    }
    public void OpenQuitScreen()
    {
        pauseScreen.SetActive(false);
        playerUIScreen.SetActive(true);
        quitScreen.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("SCN_Lobby"); // Ana menü sahnesini yükle
    }

    public void CancelQuit()
    {
        OpenPauseScreen(); // Quit menüsünden pause menüsüne dön
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
