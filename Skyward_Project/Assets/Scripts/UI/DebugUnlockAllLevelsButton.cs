using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DebugUnlockAllLevelsButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);       
    }

    private void OnClick()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
        foreach (var button in FindObjectsByType<LevelButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            button.ForceUnlock();
        }
    }
}
