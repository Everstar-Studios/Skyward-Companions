using System;
using TMPro;
using UnityEngine;

public class NameInputField : MonoBehaviour
{
    private TMP_Text nameText;
    private TMP_InputField inputField;

    private void Awake()
    {
        nameText = GetComponent<TMP_Text>();
        inputField = GetComponent<TMP_InputField>();
    }

    private void OnEnable()
    {
        if (inputField != null)
        {
            inputField.text = PlayerSystem.PlayerName;
            return;
        }

        if (nameText != null)
        {
            nameText.text = PlayerSystem.PlayerName;
        }
    }
}
