using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelEndComponent : MonoBehaviour
{
    private bool ended;

    private void OnTriggerEnter(Collider other)
    {
        var player = PlayerSystem.Player;
        if (other.gameObject != player.gameObject)
            return;
        
        GameSystem.OnLevelCompleted();
    }
}
