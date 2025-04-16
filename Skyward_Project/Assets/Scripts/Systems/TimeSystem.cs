using System;
using Skyward.Core;
using UnityEngine;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class TimeSystem : BaseSystem<TimeSystem>, ISkywardComponent
    {
        public class TimeData
        {
            public float timeAsSeconds;
            public TimeSpan timeSpan;

            public void AdvanceInTime(float deltaTime)
            {
                timeAsSeconds += deltaTime;
                timeSpan = TimeSpan.FromSeconds(timeAsSeconds);
            }
        }
        
        private readonly TimeData timeData = new();
        public static TimeSpan TimeSpan => Instance.timeData.timeSpan;
        public static float TimeInLevel => Instance.timeData.timeAsSeconds;

        private bool canUpdate;

        protected override void Initialize(GameContext context)
        {
            base.Initialize(context);

            GameInputSystem.OnMove += OnCharacterStartedMoving;
            CutsceneSystem.CutsceneStarted += CutsceneStarted;
            CutsceneSystem.CutsceneStopped += CutsceneStopped;
        }

        void ISkywardComponent.Cleanup()
        {
            CutsceneSystem.CutsceneStarted -= CutsceneStarted;
            CutsceneSystem.CutsceneStopped -= CutsceneStopped;
        }

        private void CutsceneStopped(object sender, EventArgs e)
        {
            canUpdate = true;
        }

        private void CutsceneStarted(object sender, EventArgs e)
        {
            canUpdate = false;
        }

        private void OnCharacterStartedMoving(object sender, EventArgs e)
        {
            GameInputSystem.OnMove -= OnCharacterStartedMoving;
            canUpdate = true;
        }

        private void Update()
        {
            if (!canUpdate)
                return;

            timeData.AdvanceInTime(Time.deltaTime);
        }
    }

}
