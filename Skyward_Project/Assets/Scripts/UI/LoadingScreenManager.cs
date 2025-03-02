using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public Slider progressBar;  // UI Slider for loading bar
    private string nextScene;  

    private void Start()
    {
        // Get the saved scene name (default to SCN_Lobby if not set)
        nextScene = PlayerPrefs.GetString("NextScene", "SCN_Lobby"); 
        Debug.Log("✅ Loading Scene: " + nextScene); // Check if scene name is correct
        StartCoroutine(LoadSceneAsync(nextScene)); 
    }

    IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; // Wait before activating the scene

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress; // Update progress bar

            if (operation.progress >= 0.9f)
            {
                // Wait a moment, then activate the scene
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
