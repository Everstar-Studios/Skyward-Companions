using System;
using System.Collections;
using Skyward.Characters;
using Skyward.Utils;
using UnityEngine;

public class BreakingPlatform : MonoBehaviour
{
    [SerializeField] 
    private Collider collider;
    [SerializeField] 
    private float breakDelay;

    private bool isBroken = false;

    private Transform player;

    private void OnCollisionEnter(Collision other)
    {
        if (isBroken)
            return;
        if (!other.collider.TryGetComponent(out player))
            return;

        StartCoroutine(PrepareBreak());
    }

    private IEnumerator PrepareBreak()
    {
        isBroken = true;
        yield return new WaitForSeconds(breakDelay);
        transform.GetRoot().gameObject.SetActive(false);

    }

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
}
