using System;
using Skyward.UI;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkywardButton : MonoBehaviour
{
    public ButtonAsset data;
    private Button button;

    private void Awake()
    {
        if (data == null)
            throw new Exception($"{name} do not have a button asset!!");
        
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        data.Action();
    }
}
