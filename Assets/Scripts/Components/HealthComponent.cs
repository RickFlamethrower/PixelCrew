using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PixelCrew.Components
{   
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] public  int _health;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onDie;

        
        
        public void ApplyDamage(int damageValue)
        {
            _health -= damageValue;
            _onDamage?.Invoke(); // короткая запись проверки на null
            // if (_onDamage != null)
            // _onDamage.Invoke();
            if (_health <= 0)
            {
                _onDie?.Invoke();
                
            }
        } 
    }
}
