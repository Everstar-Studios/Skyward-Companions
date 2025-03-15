using System;
using System.Collections;
using Skyward.Core;
using UnityEngine;

[RequiredSystem]
public class HeightSystem : BaseSystem<HeightSystem>, ISkywardComponent
{
    private Transform player;

    private float height;
    public static float Height => Instance.height;
    private float startingY;

    private Coroutine coroutine;
    void ISkywardComponent.WorldLoaded()
    {
        player = PlayerSystem.Player.transform;
        startingY = player.position.y;
        coroutine = Instance.StartCoroutine(UpdateHeight());
    }

    void ISkywardComponent.Cleanup()
    {
        if (coroutine != null)
        {
            Instance.StopCoroutine(coroutine);
            coroutine = null;
            
        }
    }

    private IEnumerator UpdateHeight()
    {
        while (true)
        {
            height = (player.position.y - startingY) / Configs.PlayerConfig.heightConversionFactor;
            yield return null;
        }
    }
}
