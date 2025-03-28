using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public GameObject mainMenuScreen;
    public GameObject playScreen;
    public GameObject leaderboardScreen;
    public GameObject settingsScreen;
    public GameObject creditScreen;
    public GameObject namePanel;

    private List<GameObject> menus = new();

    [SerializeField] private TMP_InputField nameField;

    private IEnumerator Start()
    {
        menus.Add(mainMenuScreen);
        menus.Add(namePanel);
        menus.Add(playScreen);
        menus.Add(leaderboardScreen);
        menus.Add(settingsScreen);
        menus.Add(creditScreen);
        menus.ForEach(g => g.SetActive(false));
        yield return new WaitUntil(() => PlayerSystem.Instance != null);
        bool hasName = !string.IsNullOrEmpty(PlayerSystem.PlayerName);
        
        if (hasName)
        {
            OpenMainMenu();
            yield break;
        }

        OpenNameScreen();
        nameField.onEndEdit.AddListener(NameCreated);
        nameField.onValidateInput += (input, charIndex, addedChar) => NameChanged(input, addedChar);
    }

    private char NameChanged(string newName, char character)
    {
        if (newName.Length > 8)
            character = '\0';

        return character;
    }

    private async void NameCreated(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;
        
        PlayerSystem.PlayerName = name;
        await AuthenticationService.Instance.UpdatePlayerNameAsync(name);
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
    
    public void OpenNameScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        namePanel.SetActive(true);
    }

    public void OpenLevel(string sceneName)
    {
        GameSystem.LaunchLevel(sceneName);
    }
}
