using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private SkywardGame game;
    public SkywardGame Game
    {
        get => game;
        set => game = value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private IEnumerator FactoryCoroutine()
    {
        while(true)
        {
            if(Game != null)
                yield return Game.Factory.ProcessQueue();
            
            yield return new WaitForFixedUpdate();
        }
    }

    public void StartGame(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
