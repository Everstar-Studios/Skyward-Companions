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

            levelTimer += Time.deltaTime;
        }
    }

}
