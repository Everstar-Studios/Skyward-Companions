using System;
using Skyward.Characters;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.UI;

[RequiredSystem]
public class UISystem : BaseSystem, ISkywardComponent
{
    private GameHUDComponent gameHUD;
    protected override void WorldLoading(GameContext context)
    {
        base.WorldLoading(context);

        gameHUD = GameManager.Instance.GameHUD;

        CutsceneSystem.CutsceneStarted += OnCutsceneStarted;
        CutsceneSystem.CutsceneStopped += OnCutsceneStopped;
        CheckpointSystem.DeathZoneReached += OnDeathZoneReached;
    }

    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        gameHUD.gameObject.SetActive(true);
    }

    private void OnDeathZoneReached(object sender, DeathZoneReachedEventArgs args)
    {
        gameHUD.FadeOutAndIn();
        args.timeToTeleportPlayer = gameHUD.fadeOutDuration;
        args.timeToReEnableInput = gameHUD.durationBetweenFade + gameHUD.fadeInDuration;
    }

    void ISkywardComponent.Cleanup()
    {
        base.Cleanup();
        
        CutsceneSystem.CutsceneStarted -= OnCutsceneStarted;
        CutsceneSystem.CutsceneStopped -= OnCutsceneStopped;
        CheckpointSystem.DeathZoneReached -= OnDeathZoneReached;
    }
    
    private void OnCutsceneStarted(object sender, EventArgs e)
    {
        var renderTexture = GameManager.Instance.GetComponentInChildren<RawImage>(true);
        renderTexture.color = new Color(1, 1, 1, 1);
        SetEnableGameHUD(false);
    }
    
    private void OnCutsceneStopped(object sender, EventArgs e)
    {
        var renderTexture = GameManager.Instance.GetComponentInChildren<RawImage>(true);
        renderTexture.color = new Color(1, 1, 1, 0);
        SetEnableGameHUD(true);
    }

    private void SetEnableGameHUD(bool enable)
    {
        gameHUD.gameObject.SetActive(enable);
    }
}
