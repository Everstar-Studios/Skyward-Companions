using System;
using System.Collections;
using Skyward.Characters;
using Skyward.Utils;
using UnityEngine;
using UnityEngine.Events;

public class BreakingPlatform : MonoBehaviour
{
    [SerializeField] 
    private Collider collider;
    [SerializeField] 
    private float breakDelay;
    [SerializeField] 
    private UnityEvent onPlayerStepEvent;
    [SerializeField] 
    private UnityEvent preBreakEvent;
    [SerializeField] 
    private float respawnDelay = 5f;

    private bool isBroken = false;

    private Transform player;

    private IEnumerator Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        yield return CheckForPlayer();
    }

    private IEnumerator CheckForPlayer()
    {
        while (true)
        {
            if (!collider.bounds.Contains(player.position))
                yield return new WaitForFixedUpdate();
            else
                yield return PrepareBreakFlow();
        }
    }
    
    private IEnumerator PrepareBreakFlow()
    {
        isBroken = true;
        onPlayerStepEvent.Invoke();
        
        yield return new WaitForSeconds(breakDelay);

        var child = collider.gameObject;
        preBreakEvent.Invoke();
        child.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        child.SetActive(true);
        isBroken = false;
    }
}
