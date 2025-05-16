using System;
using System.Collections;
using System.IO;
using Skyward.Characters;
using Skyward.Core;
using UnityEngine;

public class HeightSystem : BaseSystem<HeightSystem>, ISkywardComponent
{
    private Transform player;

    private float height;
    public static float Height => Instance.height;
    private float startingY;

    private Coroutine coroutine;

    private HeightInfo heightInfo = new();
    
    protected override void Initialize(GameContext context)
    {
        base.Initialize(context);
        
        context.Store(heightInfo);
    }
    
    protected override void WorldLoading(GameContext context)
    {
        base.WorldLoading(context);
        PlayerSystem.PlayerFound += PlayerSpawned;
    }

    private void PlayerSpawned(object sender, PlayerController e)
    {
        player = PlayerSystem.Player.transform;
        startingY = player.position.y;
        //coroutine = Instance.StartCoroutine(UpdateHeight());
    }

    void ISkywardComponent.Cleanup()
    {
        PlayerSystem.PlayerFound -= PlayerSpawned;
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
            height = (player.position.y - startingY) / ConfigSystem.GetConfig<PlayerConfig>().heightConversionFactor;
            if (height > heightInfo.highestHeight)
                heightInfo.highestHeight = height;
            
            yield return null;
        }
    }

    private class HeightInfo : ISkywardSerializable
    {
        public float highestHeight;
        public void Serialize()
        {
            PlayerPrefs.SetFloat("HeightHeight", highestHeight);
        }

        public void Deserialize()
        {
            highestHeight = PlayerPrefs.GetFloat("HighestHeight");
        }
    }
}
