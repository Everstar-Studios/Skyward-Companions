using System.Collections;
using Skyward.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    public static Lobby Instance { get; private set; }
    
    [SerializeField]
    private SkywardGame game;
    
    public GameSettings gameSettings = new();

    private IEnumerator Start()
    {
        yield return Initialize();
    }

    private IEnumerator Initialize()
    {
        yield return game.Initialize();
    }

    public void StartScene(TMP_InputField field)
    {
        SceneManager.LoadScene(field.text);
    }
}
