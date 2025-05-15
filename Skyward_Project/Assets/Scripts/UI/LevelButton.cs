using System;
using System.IO;
using System.Text.RegularExpressions;
using Skyward.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : UIButton, ISkywardComponent
{
    public bool unlockedByDefault = false;
    public GameObject lockIcon;
    
    public AssetLabelReference levelLabel;
    [SerializeField] private int levelIndex;

    private void OnEnable()
    {
        bool unlocked = GameSystem.IsLevelUnlocked(levelIndex);
        button.interactable = unlocked;
        lockIcon.SetActive(!unlocked);
    }

    public override void OnClick()
    {
        base.OnClick();
        
        GameSystem.RequestLevelLaunch(levelLabel.labelString);
    }

    public void Unlock()
    {
        bool isUnlocked = unlockedByDefault || GameSystem.IsLevelUnlocked(levelIndex);
        button.interactable = isUnlocked;
        lockIcon.SetActive(!isUnlocked);
        
    }

    public void ForceUnlock()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        unlockedByDefault = true;
        Unlock();
    }
    
    private static int ParseLevelIndex(string label)
    {
        // expects LEVEL_01, LEVEL_02 … LEVEL_10 etc.
        var match = Regex.Match(label, @"(\d+)$");
        if (!match.Success)
            throw new FormatException($"Label {label} doesn't end with digits.");
        return int.Parse(match.Value);
    }
}