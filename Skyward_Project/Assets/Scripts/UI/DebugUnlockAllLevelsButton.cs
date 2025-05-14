using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DebugUnlockAllLevelsButton : UIButton
{
    public override void OnClick()
    {
        foreach (var levelButton in FindObjectsByType<LevelButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            levelButton.ForceUnlock();
        }
    }
}
