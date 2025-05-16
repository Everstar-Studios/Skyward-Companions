using UnityEngine;

namespace Skyward.Core
{
    public abstract class BaseSystem : MonoBehaviour, ISystem
    {
        internal GameContext gamecontext;
        protected GameContext GameContext => gamecontext; 
        
        protected virtual void Initialize(GameContext context) { }
        protected virtual void PostInitialize(GameContext context) { }
        protected virtual void WorldLoading(GameContext context) { }
        protected virtual void Cleanup() { }

        void ISystem.Initialize(GameContext context)
        {
            Initialize(context);
        }

        void ISystem.PostInitialize(GameContext context)
        {
            PostInitialize(context);
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
