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
    [SerializeField]
    private SkywardGame game;
    
    public GameObject mainPanel;
    public GameObject mainMenuScreen;
    public GameObject playScreen;
    public GameObject leaderboardScreen;
    public GameObject loadingScreen;
    public GameObject settingsScreen;
    public GameObject namePanel;

    private List<GameObject> menus = new();

    [SerializeField] private TMP_InputField nameField;

    private IEnumerator Start()
    {
        yield return game.Initialize();
        
        menus.Add(mainMenuScreen);
        menus.Add(namePanel);
        menus.Add(playScreen);
        menus.Add(leaderboardScreen);
        menus.Add(loadingScreen);
        menus.Add(settingsScreen);
        menus.ForEach(g => g.SetActive(false));
        yield return new WaitUntil(() => PlayerSystem.Instance != null);
        
        GameSystem.PreLevelLoad += PreLevelLoading;
        GameSystem.LevelLoading += LevelLoading;
        GameSystem.LevelLoaded += LevelLoaded;
        
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

    private void OnDestroy()
    {
        GameSystem.PreLevelLoad -= PreLevelLoading;
        GameSystem.LevelLoading -= LevelLoading;
        GameSystem.LevelLoaded -= LevelLoaded;
    }

    private void LevelLoaded(object sender, EventArgs args)
    {
        menus.ForEach(g => g.SetActive(false));
        loadingScreen.SetActive(false);
        mainPanel.SetActive(false);
    }

    private void PreLevelLoading(object sender, EventArgs args)
    {
        menus.ForEach(g => g.SetActive(false));
        loadingScreen.SetActive(true);
    }

    private Slider loadingScreenSlider;
    private void LevelLoading(object sender, float progress)
    {
        loadingScreenSlider = loadingScreen.GetComponentInChildren<Slider>();
        loadingScreenSlider.value = progress;
    }

    private char NameChanged(string newName, char character)
    {
        if (newName.Length > 10)
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
    
    public void OpenNameScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        namePanel.SetActive(true);
    }
}
