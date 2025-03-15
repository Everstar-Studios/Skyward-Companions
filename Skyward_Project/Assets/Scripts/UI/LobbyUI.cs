using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject playScreen;
    public GameObject leaderboardScreen;
    public GameObject settingsScreen;
    public GameObject creditScreen;

    private List<GameObject> menus = new();


    private void Start()
    {
        menus.Add(mainMenuScreen);
        menus.Add(playScreen);
        menus.Add(leaderboardScreen);
        menus.Add(settingsScreen);
        menus.Add(creditScreen);
        OpenMainMenu();
    }

    public void OpenPlayScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        playScreen.SetActive(true);
    }

    public void OpenMainMenu()
    {
        menus.ForEach(g => g.SetActive(false));
        mainMenuScreen.SetActive(true);
    }

    public void OpenLeaderBoardScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        leaderboardScreen.SetActive(true);
    }
    public void OpenSettingsScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        settingsScreen.SetActive(true);
    }
    public void OpenCreditScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        creditScreen.SetActive(true);
    }

    public void OpenLevel(string sceneName)
    {
        GameSystem.LaunchLevel(sceneName);
    }
}
