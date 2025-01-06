using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Skyward.Core
{
    public interface ICoreComponent { }
    
    public interface ISystem : ICoreComponent
    {
        void Preload(GameContext context) { }
        void Initialize(GameContext context) { }
        void Cleanup() { }
    }

    public class GameContext
    {
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