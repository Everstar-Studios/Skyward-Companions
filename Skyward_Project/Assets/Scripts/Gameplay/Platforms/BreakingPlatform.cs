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
            {
                isBroken = true;
                yield return PrepareBreak();
                break;
            }
        }
    }
    
    private IEnumerator PrepareBreak()
    {
        isBroken = true;
        onPlayerStepEvent.Invoke();
        
        yield return new WaitForSeconds(breakDelay);
        
        preBreakEvent.Invoke();
        transform.GetRoot().gameObject.SetActive(false);
    }
}
