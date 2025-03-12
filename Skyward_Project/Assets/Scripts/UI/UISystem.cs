using System;
using Skyward.Core;
using Skyward.Systems;

[RequiredSystem]
public class UISystem : BaseSystem, ISkywardComponent
{
    private GameHUDComponent gameHUD;
    void ISkywardComponent.WorldLoaded()
    {
        gameHUD = ComponentSystem<GameHUDComponent>.Instance;

        CutsceneSystem.CutsceneStarted += OnCutsceneStarted;
        CutsceneSystem.CutsceneStopped += OnCutsceneStopped;
        CheckpointSystem.DeathZoneReached += OnDeathZoneReached;
    }

    private void OnDeathZoneReached(object sender, DeathZoneReachedEventArgs args)
    {
        float waitTime = gameHUD.FadeOutAndIn();
        args.waitTime = waitTime;
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
        gameHUD.gameObject.SetActive(false);
    }
    
    private void OnCutsceneStopped(object sender, EventArgs e)
    {
        gameHUD.gameObject.SetActive(true);
    }
}
