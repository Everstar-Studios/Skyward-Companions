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
        GameSystem.BackToMainMenu += BackToMainMenu;
        
        audioInstance?.Play();
    }

    private void BackToMainMenu(object sender, EventArgs args)
    {
        // Audio System releases all instances on Cleanup so we're recreating this here
        audioInstance = AudioSystem.CreateAudioInstance(audioAsset, gameObject);
        audioInstance?.Play();
    }

    private void LevelLoading(object sender, EventArgs args)
    {
        audioInstance?.Stop();
    }

    private void OnDestroy()
    {
        // is only null if application quits during splash screen load. Hacky but no time.
        if (GameSystem.Instance == null)
            return;
        
        GameSystem.PreLevelLoad -= LevelLoading;
        GameSystem.BackToMainMenu -= BackToMainMenu;
    }
}
