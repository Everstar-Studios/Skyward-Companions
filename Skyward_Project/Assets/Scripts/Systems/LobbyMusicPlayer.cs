using System;
using UnityEngine;

public class LobbyMusicPlayer : AmbientSoundPlayer
{
    private void Start()
    {
        SkywardGame.Instance.levelLoading += LevelLoading;
    }

    private void LevelLoading(AsyncOperation obj)
    {
        SkywardGame.Instance.levelLoading -= LevelLoading;
        Stop();
    }
}
