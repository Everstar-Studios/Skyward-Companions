using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Events;

public class QuestionComponent : MonoBehaviour, ISkywardComponent
{
    [Serializable]
    public class CorrectAnswerData
    {

    }

    [SerializeField]
    public string question;
    [SerializeField] 
    private float triggerRadius;
    [SerializeField]
    public Collider correctPlatform;
    [SerializeField]
    private List<BreakingPlatform> wrongPlatforms;
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
        foreach (BreakingPlatform wrongPlatform in wrongPlatforms)
        {
            if (wrongPlatform.IsBroken)
            {
                Failed();
                break;
            }
        }

        if (correctPlatform.bounds.Contains(player.position))
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
