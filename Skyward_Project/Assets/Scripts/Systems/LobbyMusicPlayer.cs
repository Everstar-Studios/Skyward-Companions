using System;
using System.Collections;
using UnityEngine;

public class LobbyMusicPlayer : AmbientSoundPlayer
{
    protected override bool CanPlayAutomatically() => false;
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameSystem.Instance != null);
        GameSystem.PreLevelLoad += LevelLoading;
        
        audioAsset.Play(transform.position);
    }

    private void LevelLoading(object sender, EventArgs args)
    {
        Stop();
    }

    private void OnDestroy()
    {
        GameSystem.PreLevelLoad -= LevelLoading;
    }
}
