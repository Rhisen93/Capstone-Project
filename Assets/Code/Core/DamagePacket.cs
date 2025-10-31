using UnityEngine;

namespace ForestSlice.Core
{
    [System.Serializable]
    public struct DamagePacket
    {
        public float amount;
        public GameObject source;
        public Vector2 hitPoint;
        public Vector2 knockbackDirection;
        public float knockbackForce;
        public DamageType type;

        public DamagePacket(float amount, GameObject source, Vector2 hitPoint)
        {
            this.amount = amount;
            this.source = source;
            this.hitPoint = hitPoint;
            this.knockbackDirection = Vector2.zero;
            this.knockbackForce = 0f;
            this.type = DamageType.Physical;
        }
    }

    public enum DamageType
    {
        Physical,
        Fire,
        Poison,
        Magic
    }
}
