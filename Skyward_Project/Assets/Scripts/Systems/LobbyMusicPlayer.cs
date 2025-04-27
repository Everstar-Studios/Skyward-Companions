using System;
using System.Collections;
using UnityEngine;

public class LobbyMusicPlayer : AmbientSoundPlayer
{
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GameSystem.Instance != null);
        GameSystem.PreLevelLoad += LevelLoading;
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
