using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Characters;
using Skyward.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuestionComponent : MonoBehaviour, ISkywardComponent
{
    [Serializable]
    public class CorrectAnswerData
    {
        public Collider platform;
    }
    
    [Serializable]
    public class WrongAnswerData
    {
        public BreakingPlatform platform;
    }

    [SerializeField]
    public string question;
    [SerializeField] 
    private float triggerRadius;
    [SerializeField]
    private CorrectAnswerData correctAnswer;
    [SerializeField]
    private List<WrongAnswerData> wrongAnswers;
    [SerializeField]
    private UnityEvent succeededEvent;
    [SerializeField]
    private UnityEvent failedEvent;
    private bool questionAsked = false;

    private bool questionAddressed = false;

    void ISkywardComponent.WorldLoaded()
    {
        StartCoroutine(RecognizePlayer());
    }

    private IEnumerator RecognizePlayer()
    {
        Transform player = PlayerSystem.Player.transform;
        
        while (!questionAddressed)
        {
            if (!IsPlayerNearby(player))
            {
                if (questionAsked)
                    OnQuestionEnded();
                
                yield return new WaitForFixedUpdate();
            }
            else
            {
                if (!questionAsked)
                    AskQuestion();
                
                CheckQuestionStatus(player);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private bool IsPlayerNearby(Transform player) => Vector3.SqrMagnitude(transform.position - player.position) < triggerRadius * triggerRadius;

    private void AskQuestion()
    {
        QuestionSystem.OnQuestionAsked(this);
        questionAsked = true;
    }
    
    private void OnQuestionEnded()
    {
        questionAsked = false;
        QuestionSystem.OnQuestionEnded();
    }

    private void CheckQuestionStatus(Transform player)
    {
        foreach (WrongAnswerData wrongAnswer in wrongAnswers)
        {
            if (wrongAnswer.platform.IsBroken)
            {
                Failed();
                break;
            }
        }

        if (correctAnswer.platform.bounds.Contains(player.position))
        {
            Succeeded();
        }
    }

    private void Failed()
    {
        failedEvent.Invoke();
        questionAddressed = true;
        OnQuestionEnded();
    }

    private void Succeeded()
    {
        succeededEvent.Invoke();
        questionAddressed = true;
        OnQuestionEnded();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
