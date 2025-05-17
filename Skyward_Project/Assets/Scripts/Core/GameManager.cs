using System.Collections;
using Skyward.Core;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour, ISkywardComponent
{
    private GameHUDComponent gameHUD;
    public GameHUDComponent GameHUD => gameHUD;
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

        gameHUD = GetComponentInChildren<GameHUDComponent>(true);
        gameHUD.gameObject.SetActive(false);
    }

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        game = context.game;
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

    public static void CutsceneStarted(RenderTexture renderTexture)
    {
        Instance.gameHUD.cutsceneRawImage.texture = renderTexture;
        Instance.gameHUD.cutsceneRawImage.gameObject.SetActive(true);
    }
    
    public static void CutsceneEnded()
    {
        Instance.gameHUD.cutsceneRawImage.texture = null;
        Instance.gameHUD.cutsceneRawImage.gameObject.SetActive(false);
    }
}
