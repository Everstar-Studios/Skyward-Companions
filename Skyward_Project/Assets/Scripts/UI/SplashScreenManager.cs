using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public string nextSceneName = "SCN_SplashLoadingScene";
    public Slider loadingSlider;
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
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        var async = SceneManager.LoadSceneAsync(nextSceneName);
        async.allowSceneActivation = false;
        loadingSlider.transform.parent.gameObject.SetActive(true);
        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / 0.9f);
            loadingSlider.value = progress;
            
            if (async.progress >= 0.9f)
            {
                yield return new WaitForSeconds(waitTime);
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
