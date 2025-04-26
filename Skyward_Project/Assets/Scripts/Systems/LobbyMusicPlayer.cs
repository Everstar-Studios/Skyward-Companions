using System;
using UnityEngine;

public class LobbyMusicPlayer : AmbientSoundPlayer
{
    private void Start()
    {
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
