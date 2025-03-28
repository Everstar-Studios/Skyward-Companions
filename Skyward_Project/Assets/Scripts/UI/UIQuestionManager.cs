using System;
using Skyward.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIQuestionManager : MonoBehaviour, ISkywardComponent
{
    [SerializeField]
    private TMP_Text questionText;
    
    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        QuestionSystem.QuestionAsked += OnQuestionAsked;
        QuestionSystem.QuestionEnded += OnQuestionEnded;
    }
    void ISkywardComponent.Cleanup()
    {
        QuestionSystem.QuestionAsked -= OnQuestionAsked;
        QuestionSystem.QuestionEnded -= OnQuestionEnded;
    }

    private void OnQuestionEnded(object sender, EventArgs args)
    {
        questionText.gameObject.SetActive(false);
        questionText.text = String.Empty;
    }


    private void OnQuestionAsked(object sender, QuestionSystem.QuestionArgs args)
    {
        questionText.gameObject.SetActive(true);
        questionText.text = args.question;
    }
}
