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
    public float divident = 3f;
    void ISkywardComponent.WorldLoaded()
    {
        player = PlayerSystem.Player.transform;
        startingY = player.position.y;
        Instance.StartCoroutine(UpdateHeight());
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
