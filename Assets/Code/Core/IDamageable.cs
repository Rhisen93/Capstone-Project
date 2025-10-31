using UnityEngine;

namespace ForestSlice.Core
{
    public interface IDamageable
    {
        void TakeDamage(DamagePacket packet);
        bool IsAlive { get; }
        Transform GetTransform();
    }
}
