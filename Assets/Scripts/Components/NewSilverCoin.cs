using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Components
{
    public class NewSilverCoin : MonoBehaviour
    {
        [SerializeField] private int _silverCoinPoint = 2;

        public void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                HeroMove.Session.Data.Coins += _silverCoinPoint;
                Destroy(gameObject);
            }
            
        }

    }
}

