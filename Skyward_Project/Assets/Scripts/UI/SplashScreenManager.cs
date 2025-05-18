using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public string nextSceneName = "SCN_SplashLoadingScene";
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public float waitTime;

    private CutsceneInputAction inputAction;

    void Start()
    {
        inputAction = new();
        inputAction.Enable();
        inputAction.Cutscene.SkipCutscene.performed += CutsceneSkipped;
        
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void CutsceneSkipped(InputAction.CallbackContext obj)
    {
        videoPlayer.Stop();
        OnVideoEnd(videoPlayer);
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
        inputAction.Cutscene.SkipCutscene.performed -= CutsceneSkipped;
        inputAction.Disable();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(PrepareGame());
    }

    private IEnumerator PrepareGame()
    {
        loadingSlider.transform.parent.gameObject.SetActive(true);
        yield return LoadCatalog();
        yield return LoadLobbyScene();

        yield return new WaitForSeconds(waitTime);
    }

    private IEnumerator LoadLobbyScene()
    {
        var sceneHandle = SceneManager.LoadSceneAsync(nextSceneName);
        sceneHandle.allowSceneActivation = false;
        loadingText.text = "Entering the world...";
        while (!sceneHandle.isDone)
        {
            float progress = Mathf.Clamp01(sceneHandle.progress / 0.9f);
            loadingSlider.value = progress;
            
            if (sceneHandle.progress >= 0.9f)
            {
                yield return new WaitForSeconds(waitTime);
                sceneHandle.allowSceneActivation = true;
                break;
            }
            
            yield return null;
        }
    }

    private IEnumerator LoadCatalog()
    {
        string catalogUrl = DeliveryBucketManager.GetContentCatalogURL(BucketEnvironment.Development);
        var catalogHandle = Addressables.LoadContentCatalogAsync(catalogUrl);
        loadingText.text = "Preparing game content...";
        while (!catalogHandle.IsDone)
        {
            float progress = Mathf.Clamp01(catalogHandle.PercentComplete / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }

        if (catalogHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load content catalog: " + catalogHandle.OperationException);
            Addressables.Release(catalogHandle);
            yield break;
        }

        Addressables.Release(catalogHandle);
        Debug.Log($"Remote catalog loaded: {catalogUrl}");
    }
}
