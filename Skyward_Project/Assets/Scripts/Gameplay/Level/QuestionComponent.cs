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
        public string answer;
    }
    
    [Serializable]
    public class WrongAnswerData
    {
        public BreakingPlatform platform;
        public string answer;
    }

    [SerializeField] 
    private Collider trigger;
    [SerializeField]
    private CorrectAnswerData correctAnswer;
    [SerializeField]
    private List<WrongAnswerData> wrongAnswers;
    [SerializeField]
    private UnityEvent succeededEvent;
    [SerializeField]
    private UnityEvent failedEvent;
    private bool questionAsked = false;

    private void Start()
    {
        SetupAnswers();
        SetActivateQuestions(false);
    }

    private void SetupAnswers()
    {
        GetTextComponent(correctAnswer.platform).text = correctAnswer.answer;
        wrongAnswers.ForEach((w) => GetTextComponent(w.platform).text = w.answer);
    }

    private TMP_Text GetTextComponent(Component answer)
    {
        return answer.GetComponentInChildren<TMP_Text>();
    }

    private void SetActivateQuestions(bool active)
    {
        correctAnswer.platform.gameObject.SetActive(active);
        SetActivateWrongPlatforms(active);
    }

    private void SetActivateWrongPlatforms(bool active)
    {
        wrongAnswers.ForEach((w) => w.platform.gameObject.SetActive(active));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (questionAsked)
            return;
        if (other.transform != PlayerSystem.Player.transform)
            return;

        ShowQuestions();
    }

    private void ShowQuestions()
    {
        SetActivateQuestions(true);
        StartCoroutine(OnQuestionAsked());
    }

    private IEnumerator OnQuestionAsked()
    {
        questionAsked = true;
        Transform player = PlayerSystem.Player.transform;
        while (true)
        {
            foreach (WrongAnswerData wrongAnswer in wrongAnswers)
            {
                if (wrongAnswer.platform.IsBroken)
                {
                    Failed();
                    yield break;
                }
            }

            if (correctAnswer.platform.bounds.Contains(player.position))
            {
                Succeeded();
                yield break;
            }
            
            yield return new WaitForFixedUpdate();
        }
    }

    private void Failed()
    {
        failedEvent.Invoke();
        SetActivateQuestions(false);
    }

    private void Succeeded()
    {
        succeededEvent.Invoke();
        SetActivateWrongPlatforms(false);
    }
}
