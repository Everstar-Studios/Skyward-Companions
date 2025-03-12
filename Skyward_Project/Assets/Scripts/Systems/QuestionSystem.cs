using System;
using Skyward.Characters;
using Skyward.Core;
using Unity.VisualScripting;
using UnityEngine;

[RequiredSystem]
public class QuestionSystem : BaseSystem<QuestionSystem>
{
    public static event EventHandler<QuestionArgs> QuestionAsked
    {
        add => Instance.questionAsked += value;
        remove => Instance.questionAsked -= value;
    }

    private event EventHandler<QuestionArgs> questionAsked;
    
    public static event EventHandler QuestionEnded
    {
        add => Instance.questionEnded += value;
        remove => Instance.questionEnded -= value;
    }

    private event EventHandler questionEnded;

    public class QuestionArgs : EventArgs
    {
        public string question;
    }

    public static void OnQuestionAsked(QuestionComponent questionComponent)
    {
        var args = new QuestionArgs()
        {
            question = questionComponent.question
        };

        Instance.questionAsked?.Invoke(Instance, args);
    }
    
    public static void OnQuestionEnded()
    {
        Instance.questionEnded?.Invoke(Instance, EventArgs.Empty);
    }
}
