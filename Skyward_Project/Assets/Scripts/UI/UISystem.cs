using System;
using Skyward.Core;
using UnityEngine;


[RequiredSystem]
public class UISystem : BaseSystem, ISkywardComponent
{
    private GameHUDComponent gameHUD;
    void ISkywardComponent.WorldLoaded()
    {
        gameHUD = ComponentSystem<GameHUDComponent>.Instance;

        CutsceneSystem.CutsceneStarted += OnCutsceneStarted;
        CutsceneSystem.CutsceneStopped += OnCutsceneStopped;
    }

    void ISkywardComponent.Cleanup()
    {
        base.Cleanup();
        
        CutsceneSystem.CutsceneStarted -= OnCutsceneStarted;
        CutsceneSystem.CutsceneStopped -= OnCutsceneStopped;
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
