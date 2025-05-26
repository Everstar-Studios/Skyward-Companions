using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUDComponent : MonoBehaviour
{
    public GameObject pauseScreen;
    public GameObject playerUIScreen;
    public GameObject quitScreen;
    public GameObject settingsScreen;
    public GameObject levelEndScreen;
    public RawImage cutsceneRawImage;
    
    public CanvasGroup canvasGroup;
    public Image panel;
    public float fadeOutDuration = 0.5f;
    public float fadeInDuration = 1.0f;
    public float durationBetweenFade = 1f;
    
    private List<GameObject> menus = new();

    private void Awake()
    {
        menus.Add(playerUIScreen);
        menus.Add(pauseScreen);
        menus.Add(quitScreen);
        menus.Add(settingsScreen);
        menus.Add(levelEndScreen);
    }

    private void Start()
    {
        GameSystem.LevelCompleted += OnLevelCompleted;
    }

    private void OnDestroy()
    {
        GameSystem.LevelCompleted -= OnLevelCompleted;
    }

    private void OnLevelCompleted(object sender, GameSystem.LevelEndEventArgs args)
    {
        OpenLevelEndScreen();
    }

    private void OnEnable()
    {
        OpenPlayerUIScreen();
    }

    public void OpenPlayerUIScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        playerUIScreen.SetActive(true);
        GameSystem.OnGameUnpaused();
    }
    public void OpenPauseScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        pauseScreen.SetActive(true);
        GameSystem.OnGamePaused();
    }
    public void OpenQuitScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        quitScreen.SetActive(true);
    }

    public void OpenSettingsScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        settingsScreen.SetActive(true);
        GameSystem.OnGamePaused();
    }

    private void OpenLevelEndScreen()
    {
        menus.ForEach(g => g.SetActive(false));
        levelEndScreen.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        GameSystem.MainMenu();
    }

    public void FadeOutAndIn()
    {
        StartCoroutine(FadeLoop());
    }

    public void RestartFromCheckpoint()
    {
        OpenPlayerUIScreen();
        CheckpointSystem.RespawnFromLastCheckpoint();
    }

    IEnumerator FadeLoop()
    {
        yield return Fade(1f, 0f, fadeOutDuration);
        yield return new WaitForSeconds(durationBetweenFade);
        yield return Fade(0f, 1f, fadeInDuration);
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            canvasGroup.alpha = alpha;
            
            float panelAlpha = Mathf.Lerp(endAlpha, startAlpha, elapsedTime / duration);
            Color color = panel.color;
            color.a = panelAlpha;
            panel.color = color;
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
}
