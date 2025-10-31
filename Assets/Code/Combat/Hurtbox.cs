using UnityEngine;
using ForestSlice.Core;

namespace ForestSlice.Combat
{
    [RequireComponent(typeof(Collider2D))]
    public class Hurtbox : MonoBehaviour
    {
        private IDamageable damageable;

        private void Awake()
        {
            damageable = GetComponentInParent<IDamageable>();
            
            if (damageable == null)
            {
                Debug.LogWarning($"Hurtbox on {gameObject.name} has no IDamageable component in parent!");
            }

            Collider2D col = GetComponent<Collider2D>();
            col.isTrigger = true;
        }

        public IDamageable GetDamageable()
        {
            return damageable;
        }
    }
}
