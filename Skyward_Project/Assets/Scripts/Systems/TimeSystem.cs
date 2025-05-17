using System;
using Skyward.Core;
using UnityEngine;

namespace Skyward.Systems
{
    [RequiredSystem]
    public class TimeSystem : BaseSystem<TimeSystem>
    {
        [Serializable]
        public class TimeData
        {
            public float timeAsSeconds;
            public TimeSpan timeSpan;

            internal void Advance(float deltaTime)
            {
                timeAsSeconds += deltaTime;
                timeSpan = TimeSpan.FromSeconds(timeAsSeconds);
            }

            internal void Reset()
            {
                timeAsSeconds = 0f;
                timeSpan = TimeSpan.Zero;
            }
        }
        
        private readonly TimeData timeData = new();
        public static TimeSpan TimeSpan => Instance.timeData.timeSpan;
        public static float TimeInLevel => Instance.timeData.timeAsSeconds;

        private bool cutscenePlaying;
        private bool startedMoving;

        private bool levelStarted;

        private void BackToMainMenu(object sender, EventArgs args)
        {
            levelStarted = false;
            timeData.Reset();
        }

        protected override void WorldLoading(GameContext context)
        {
            base.WorldLoading(context);

            GameInputSystem.OnMove += OnCharacterStartedMoving;
            CutsceneSystem.CutsceneStarted += CutsceneStarted;
            CutsceneSystem.CutsceneStopped += CutsceneStopped;
            GameSystem.BackToMainMenu += BackToMainMenu;
        }

        protected override void WorldLoaded(GameContext context)
        {
            base.WorldLoaded(context);
            
            levelStarted = true;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            
            CutsceneSystem.CutsceneStarted -= CutsceneStarted;
            CutsceneSystem.CutsceneStopped -= CutsceneStopped;
            GameSystem.BackToMainMenu -= BackToMainMenu;
        }

        private void CutsceneStopped(object sender, EventArgs e)
        {
            cutscenePlaying = false;
        }

        private void CutsceneStarted(object sender, EventArgs e)
        {
            cutscenePlaying = true;
        }

        private void OnCharacterStartedMoving(object sender, EventArgs e)
        {
            GameInputSystem.OnMove -= OnCharacterStartedMoving;
            startedMoving = true;
        }

        private void Update()
        {
            if (!levelStarted)
                return;
            if (!startedMoving || cutscenePlaying)
                return;

            timeData.Advance(Time.deltaTime);
        }
    }

}
