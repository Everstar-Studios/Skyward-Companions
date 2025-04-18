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
        void Initialize(GameContext context) { }
        void OnWorldLoading(GameContext context) { }
        void Cleanup() { }
    }

    public class GameContext
    {
        public SkywardGame game;
        
        private SaveFile saveFile;

        public GameContext(SkywardGame game)
        {
            this.game = game;
            saveFile = new SaveFile();
        }

        public void AddInstruction(FactoryInstruction instruction)
        {
            game.Factory.AddInstruction(instruction);
        }
        
        public void Store(ISkywardSerializable value)
        {
            saveFile.Store(value);
        }
        
        public void Save()
        {
            saveFile.Save();
        }

        public void Load()
        {
            saveFile.Load();
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