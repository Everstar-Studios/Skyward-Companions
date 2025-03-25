using System;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(VideoPlayer))]
public class SplashScreenManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public string nextSceneName = "SCN_SplashLoadingScene";
    public Slider loadingSlider;
    public float waitTime;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();
            OnVideoEnd(videoPlayer);
        }
    }
}
