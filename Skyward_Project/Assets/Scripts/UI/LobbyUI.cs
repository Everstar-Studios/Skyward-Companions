using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject playScreen;
    public GameObject leaderboardScreen;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenPlayScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(true);
        leaderboardScreen.SetActive(false);

    }

    public void OpenMainMenu()
    {
        mainMenuScreen.SetActive(true);
        playScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }

    public void OpenLeaderBoardScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
    }

    public void OpenLevel(string sceneName)  // Changed from int to string
    {
        PlayerPrefs.SetString("NextScene", sceneName); // Store the next scene name
        SceneManager.LoadScene("SCN_LoadingScene"); // Load the loading scene first
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
