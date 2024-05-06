using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Components
{
    public class CoinSpawnComponent : MonoBehaviour
    {
        [SerializeField] public int _countSilverCoins;
        [SerializeField] private Transform _target;
        [SerializeField] private GameObject _prefab;
        
        
        public void SpawnCoins()
        {
            var numSilverCoinsToDispose = Random.Range(2, _countSilverCoins);
            while (numSilverCoinsToDispose > 0)
            {
                var instantiate = Instantiate(_prefab, _target.position, Quaternion.identity);
               
                numSilverCoinsToDispose--;
            }     
        }
    }
}

