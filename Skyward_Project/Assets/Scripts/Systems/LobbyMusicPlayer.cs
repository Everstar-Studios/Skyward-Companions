using System;
using System.Collections;
using UnityEngine;

public class LobbyMusicPlayer : AmbientSoundPlayer
{
    protected override bool CanPlayAutomatically() => false;
    protected override IEnumerator Start()
    {
        yield return base.Start();
        yield return new WaitUntil(() => GameSystem.Instance != null);
        GameSystem.PreLevelLoad += LevelLoading;
        
        audioInstance?.Play();
    }

    private void LevelLoading(object sender, EventArgs args)
    {
        audioInstance?.Stop();
    }

    private void OnDestroy()
    {
        GameSystem.PreLevelLoad -= LevelLoading;
    }
}
