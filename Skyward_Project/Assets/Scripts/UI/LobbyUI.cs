using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject playScreen;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenPlayScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(true);
    }

    public void OpenMainMenu()
    {
        mainMenuScreen.SetActive(true);
        playScreen.SetActive(false);
    }

    public void OpenLevel(int sceneIndex)
    {
        SceneManager.LoadScene("SCN_LevelSelect"); // Level seçim ekranına yönlendir
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
