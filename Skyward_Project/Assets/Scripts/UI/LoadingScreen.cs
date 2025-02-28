using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // Progress bar UI
    public string nextScene = "SCN_Lobby"; // Lobby'e geçiş yapacak

    void Start()
    {
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        while (!operation.isDone)
        {
            progressBar.value = Mathf.Clamp01(operation.progress / 0.9f +3f); // + 3 ekledim cunku cok hizli yukleniyor test icin.
            yield return null;
        }
    }
}
