using UnityEngine;

namespace Skyward.Core
{
    public abstract class BaseSystem : MonoBehaviour, ISystem
    {
        internal GameContext gamecontext;
        protected GameContext GameContext => gamecontext; 
        
        protected virtual void Preload(GameContext context) { }
        protected virtual void WorldLoading(GameContext context) { }
        protected virtual void Cleanup() { }

        void ISystem.Preload(GameContext context)
        {
            Preload(context);
        }
        
        void ISystem.OnWorldLoading(GameContext context)
        {
            WorldLoading(context);
        }

        void ISystem.Cleanup()
        {
            Cleanup();
        }
    }

    public abstract class BaseSystem<T> : BaseSystem where T : BaseSystem
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this as T;
        }
    }

}
