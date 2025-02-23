using System;
using System.Collections.Generic;
using Skyward.Characters;
using UnityEngine;

public class QuestionComponent : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController player))
            return;
    }
}
