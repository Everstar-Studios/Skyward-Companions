using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LevelEndComponent : MonoBehaviour, ISkywardComponent
{
    private bool ended;

    private Coroutine coroutine;
    
    void ISkywardComponent.WorldLoaded(GameContext context)
    {
        coroutine = StartCoroutine(CheckForPlayer());
    }
    void ISkywardComponent.Cleanup()
    {
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator CheckForPlayer()
    {
        yield return new WaitUntil(() => PlayerSystem.Player != null);
        
        var player = PlayerSystem.Player;
        while (true)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < 1f)
            {
                GameSystem.OnLevelCompleted();
                yield break;
            }

            yield return null;
        }
    }

}
