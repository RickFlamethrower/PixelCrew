using System.Collections;
using System.Collections.Generic;
using PixelCrew.Components;
using UnityEngine;
using UnityEngine.UI;

namespace PixelCrew.Components
{
    public class SilverCoinTrigger : MonoBehaviour
    {
    
    [SerializeField] private int _silverCoinPoint = 2;
    private void OnTriggerEnter2D(Collider2D collision)
         {
            if (collision.CompareTag("Player"))
            {
                 HeroMove.Session.Data.Coins += _silverCoinPoint;
                Destroy(gameObject);
            }
         }
    }
}


