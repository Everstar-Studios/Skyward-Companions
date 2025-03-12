using System.Collections;
using Skyward.Core;
using UnityEngine;

public class GameManager : MonoBehaviour, ISkywardComponent
{
    public GameObject gameHUD;
    
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

        gameHUD.SetActive(false);
    }

    void ISkywardComponent.WorldLoaded()
    {
        gameHUD.SetActive(true);
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
}
