using System;
using Skyward.Core;
using Skyward.Systems;
using UnityEngine;
using UnityEngine.UI;

[RequiredSystem]
public class UISystem : BaseSystem, ISkywardComponent
{
    private GameHUDComponent gameHUD;
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);

        CutsceneSystem.CutsceneStarted += OnCutsceneStarted;
        CutsceneSystem.CutsceneStopped += OnCutsceneStopped;
        CheckpointSystem.DeathZoneReached += OnDeathZoneReached;
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
        GameManager.SetEnableGameHUD(false);
    }
    
    private void OnCutsceneStopped(object sender, EventArgs e)
    {
        var renderTexture = GameManager.Instance.GetComponentInChildren<RawImage>(true);
        renderTexture.color = new Color(1, 1, 1, 0);
        GameManager.SetEnableGameHUD(true);
    }
}
