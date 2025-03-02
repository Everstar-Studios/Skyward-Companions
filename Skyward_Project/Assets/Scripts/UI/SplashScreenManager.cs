using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SplashScreenManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public Slider progressBar;  // **Loading ekranında slider için**
    public string nextSceneName = "SCN_Lobby"; // Sonraki sahne adı
    public float waitingTime = 1.0f; // **Loading ekranı için bekleme süresi**

    private bool isVideoPlaying = false;

    void Start()
    {
        if (videoPlayer != null && videoPlayer.clip != null) // **Eğer Video varsa**
        {
            isVideoPlaying = true;
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
        else // **Eğer Video yoksa, doğrudan yükleme ekranı çalışsın**
        {
            if (progressBar != null) 
            {
                StartCoroutine(LoadSceneWithProgress(nextSceneName));
            }
            else // **Splash Screen'deysek beklemeden direkt geç**
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(nextSceneName); // **Video bitince direkt geç**
    }

    IEnumerator LoadSceneWithProgress(string sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; // Sahne hemen açılmasın

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress; // **Slider Güncelle**

            if (operation.progress >= 0.9f) // **Yükleme tamamlandıysa**
            {
                yield return new WaitForSeconds(waitingTime); // **Editörden ayarlanabilen bekleme süresi**
                operation.allowSceneActivation = true; // **Sahneye geç**
            }

            yield return null;
        }
    }
}
