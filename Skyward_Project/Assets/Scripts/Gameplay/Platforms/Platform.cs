using System;
using System.Collections;
using Skyward.Characters;
using UnityEngine;

public abstract class Platform : MonoBehaviour
{
    protected Collider Collider;
    protected bool playerLanded;

    private void Awake()
    {
        Collider = GetComponentInChildren<Collider>();
    }

    protected virtual IEnumerator Start()
    {
        if (Collider != null)
            yield return CheckPlayer();

        yield break;
    }

    protected IEnumerator CheckPlayer()
    {
        yield return new WaitUntil(() => PlayerSystem.Player != null);
        var player = PlayerSystem.Player;
        var playerTransform = player.transform;
        while (true)
        {
            if (!Collider.bounds.Contains(playerTransform.position))
            {
                if (playerLanded)
                    yield return OnPlayerExited(player);
                
                playerLanded = false;
                yield return new WaitForFixedUpdate();
            }
            else
            {
                playerLanded = true;
                yield return OnPlayerLanded(player);
            }
        }
    }

    protected virtual IEnumerator OnPlayerLanded(PlayerController player)
    {
        yield break;
    }

    protected virtual IEnumerator OnPlayerExited(PlayerController player)
    {
        yield break;
    }
}
