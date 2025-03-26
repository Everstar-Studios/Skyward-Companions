using System;
using System.Collections;
using Skyward.Characters;
using UnityEngine;

public class Trampoline : Platform
{
    public float force = 100f;

    protected override IEnumerator OnPlayerLanded(PlayerController player)
    {
        yield return player.GetComponent<LocomotionController>().ForceJump(force);
    }
}
