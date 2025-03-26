using System;
using System.Collections;
using Skyward.Characters;
using UnityEngine;

public class Trampoline : Platform
{
    public float force = 100f;

    protected override IEnumerator Start()
    {
        yield return base.Start();
        yield return new WaitUntil(() => PlayerSystem.Player != null);
    }

    protected override IEnumerator OnPlayerLanded(PlayerController player)
    {
        yield return player.GetComponent<LocomotionController>().ForceJump(force);
    }

    private void OnApplyExternalForce(object sender, ExternalForceArgs e)
    {
        Vector3 force = ApplyForce(e.velocity);
        e.velocity += force;
    }

    private Vector3 ApplyForce(Vector3 velocity)
    {
        if (!playerLanded)
            return Vector3.zero;
        return transform.up * force;
    }
}
