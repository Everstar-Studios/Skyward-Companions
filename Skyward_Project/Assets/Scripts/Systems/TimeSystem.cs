using System;
using Skyward.Core;
using UnityEngine;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class TimeSystem : BaseSystem<TimeSystem>, ISkywardComponent
    {
        private float levelTimer;
        public static float LevelTimer => Instance.levelTimer;

        private bool worldLoaded;
        void ISkywardComponent.WorldLoaded(GameContext context)
        {
            worldLoaded = true;
        }

        private void Update()
        {
            if (!worldLoaded)
                return;

            levelTimer += Time.deltaTime;
        }
    }

}
