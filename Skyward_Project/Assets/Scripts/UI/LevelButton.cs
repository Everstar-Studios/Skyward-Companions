using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelButton : UIButton
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
}