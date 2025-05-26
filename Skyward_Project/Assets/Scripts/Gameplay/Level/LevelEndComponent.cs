using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class LevelEndComponent : MonoBehaviour
{
    public UnityEvent levelEndEvent;
    
    private bool ended;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != PlayerSystem.Player.gameObject)
            return;
        
        levelEndEvent.Invoke();
        GameSystem.OnLevelCompleted();
    }
}
