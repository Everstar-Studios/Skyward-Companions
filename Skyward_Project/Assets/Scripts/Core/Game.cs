using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyward.Core
{
    public interface ICoreComponent { }

    public interface ISkywardComponent : ICoreComponent
    {
        void WorldLoaded(GameContext context) {}
        void Cleanup() { }
    }
    
    public interface ISystem : ICoreComponent
    {
        void Preload(GameContext context) { }
        void Initialize(GameContext context) { }
        void Cleanup() { }
    }

    public class GameContext
    {
        public SkywardGame game;

        public GameContext(SkywardGame game)
        {
            this.game = game;
        }

        public void AddInstruction(FactoryInstruction instruction)
        {
            game.Factory.AddInstruction(instruction);
        }
    }
    
    [System.Serializable]
    public class GameSettings
    {
    }

    
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredSystemAttribute : System.Attribute
    { 
        
    }
}