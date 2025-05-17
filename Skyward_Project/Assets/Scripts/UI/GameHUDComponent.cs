using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUDComponent : MonoBehaviour, ISkywardComponent
{
    public GameObject pauseScreen;
    public GameObject playerUIScreen;
    public GameObject quitScreen;
    public GameObject settingsScreen;
    public RawImage cutsceneRawImage;
    
    public CanvasGroup canvasGroup;
    public Image panel;
    public float fadeOutDuration = 0.5f;
    public float fadeInDuration = 1.0f;
    public float durationBetweenFade = 1f;

    private void Start()
    {
        OpenPlayerUIScreen();
    }

    public void OpenPlayerUIScreen()
    {
        playerUIScreen.SetActive(true);
        pauseScreen.SetActive(false);
        quitScreen.SetActive(false);        
        settingsScreen.SetActive(false);
    }
    public void OpenPauseScreen()
    {
        pauseScreen.SetActive(true);
        quitScreen.SetActive(false);
        playerUIScreen.SetActive(false);
        settingsScreen.SetActive(false);
    }
    public void OpenQuitScreen()
    {
        pauseScreen.SetActive(false);
        playerUIScreen.SetActive(false);
        quitScreen.SetActive(true);
        settingsScreen.SetActive(false);
    }

    public void OpenSettingsScreen()
    {
        pauseScreen.SetActive(false);
        playerUIScreen.SetActive(false);
        quitScreen.SetActive(false);
        settingsScreen.SetActive(true);
    }

    public void QuitToMainMenu()
    {
        GameSystem.MainMenu();
    }

    public void FadeOutAndIn()
    {
        StartCoroutine(FadeLoop());
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
