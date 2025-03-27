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
        
        public void Store(string key, ISkywardSerializable value)
        {
            saveFile.Store(key, value);
        }
        
        public T Retrieve<T>(string key) where T : ISkywardSerializable
        {
            return (T)saveFile.Retrieve(key);
        }
        
        public void Save()
        {
            saveFile.Save(Application.persistentDataPath + "/save.sky");
        }

        public void Load()
        {
            saveFile.Load(Application.persistentDataPath + "/save.sky");
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