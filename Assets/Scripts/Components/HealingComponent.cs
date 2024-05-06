using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.Components
{
    public class HealingComponent : MonoBehaviour
    {
        [SerializeField] private int _healing;
        
        private void OnTriggerEnter2D(Collider2D collision)
         {
            if (collision.CompareTag("Player"))
            {
                var HealthComponent = collision.GetComponent<HealthComponent>();
                var DamageComponent = collision.GetComponent<DamageComponent>();
                
                if (_healing != 0)
                {
                    HealthComponent._health += _healing;
                }
                else if (DamageComponent._damage != 0)
                {
                    
                    HealthComponent._health -= DamageComponent._damage;
                }
            }
         }
    }
}