using System;
using System.Collections;
using System.Collections.Generic;
using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

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

    private Transform player;

    private void Start()
    {
        SetActivateQuestions(false);
    }

    void ISkywardComponent.WorldLoaded()
    {
        player = PlayerSystem.Player.transform;
    }

    private void SetActivateQuestions(bool active)
    {
        correctAnswer.platform.gameObject.SetActive(active);
        wrongAnswers.ForEach((w) => w.platform.gameObject.SetActive(active));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != player.transform)
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
        while (true)
        {
            foreach (WrongAnswerData wrongAnswer in wrongAnswers)
            {
                if (wrongAnswer.platform.IsBroken)
                {
                    SetActivateQuestions(false);
                    yield break;
                }
            }

            if (correctAnswer.platform.bounds.Contains(player.position))
            {
                Debug.Log($"You won!");
                yield break;
            }
            
            yield return new WaitForFixedUpdate();
        }
    }
}
