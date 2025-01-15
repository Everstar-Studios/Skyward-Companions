using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    [SerializeField]
    private SkywardGame game;
    
    public GameSettings gameSettings = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        yield return Initialize();
    }

    private IEnumerator Initialize()
    {
        yield return game.Initialize(gameSettings);
    }

    public void StartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
