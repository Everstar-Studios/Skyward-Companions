using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public GameSettings gameSettings = new();
    
    private SkywardGame game;

    private void Awake()
    {
        game = GetComponent(typeof(SkywardGame)) as SkywardGame;
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
