using System;
using Skyward.Core;
using Skyward.Systems;

[RequiredSystem]
public class UISystem : BaseSystem, ISkywardComponent
{
    private GameHUDComponent gameHUD;
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);
        gameHUD = ComponentSystem<GameHUDComponent>.Instance;

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
        GameManager.SetEnableGameHUD(false);
    }
    
    private void OnCutsceneStopped(object sender, EventArgs e)
    {
        GameManager.SetEnableGameHUD(true);
    }
}
