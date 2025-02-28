using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject playScreen;
    public GameObject leaderboardScreen;
    public GameObject settingsScreen;
    public GameObject creditScreen;


    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenPlayScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(true);
        leaderboardScreen.SetActive(false);
        settingsScreen.SetActive(false);
        creditScreen.SetActive(false);

    }

    public void OpenMainMenu()
    {
        mainMenuScreen.SetActive(true);
        playScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        settingsScreen.SetActive(false);
        creditScreen.SetActive(false);
    }

    public void OpenLeaderBoardScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        leaderboardScreen.SetActive(true);
        settingsScreen.SetActive(false);
        creditScreen.SetActive(false);
    }
    public void OpenSettingsScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        settingsScreen.SetActive(true);
        creditScreen.SetActive(false);
    }
    public void OpenCreditScreen()
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
        settingsScreen.SetActive(false);
        creditScreen.SetActive(true);
    }

    public void OpenLevel(int sceneName)  // Changed from int to string
    {
        PlayerPrefs.SetInt("NextScene", sceneName); // Store the next scene name
        SceneManager.LoadScene("SCN_LoadingSceen"); // Load the loading screen first
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
