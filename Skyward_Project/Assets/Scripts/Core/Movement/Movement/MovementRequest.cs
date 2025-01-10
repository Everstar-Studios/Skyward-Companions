using UnityEngine;

namespace Skyward.Movement
{
    public abstract class MovementRequest
    {
        public abstract bool Done { get; }
        public abstract Vector3 Evaluate(MovementContext context, float totalTime);
    }
}
