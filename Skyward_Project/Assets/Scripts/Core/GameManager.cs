using System.Collections;
using Skyward.Core;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour, ISkywardComponent
{
    public GameObject gameHUD;
    private bool gameHUDDisabled;
    
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

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        if (!Instance.gameHUDDisabled)
            gameHUD.SetActive(true);

        game = context.game;

        StartCoroutine(FactoryCoroutine());
    }

    public static void SetEnableGameHUD(bool enable)
    {
        Instance.gameHUDDisabled = !enable;
        Instance.gameHUD.SetActive(enable);
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
