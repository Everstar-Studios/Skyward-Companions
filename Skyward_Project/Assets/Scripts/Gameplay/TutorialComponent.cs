using System;
using UnityEngine;

public class TutorialComponent : MonoBehaviour
{
    public GameObject tutorial;
    
    public void OpenTutorial()
    {
        if (PlayerPrefs.GetInt("Tutorial") == 1)
            return;

        tutorial.SetActive(true);
        PlayerPrefs.SetInt("Tutorial", 1);
    }
}
