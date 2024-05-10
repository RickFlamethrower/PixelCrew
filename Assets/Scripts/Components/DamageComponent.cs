using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Components
{
    public class DamageComponent : MonoBehaviour
    {
        [SerializeField] public int _damage;

        public void ApplyDamage(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.ModifyHealth(_damage);
            }
        }
    }
}
