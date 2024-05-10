using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldenColinTrigger : MonoBehaviour
{
    
    [SerializeField] private int _goldenCoinPoint = 10;
    private void OnTriggerEnter2D(Collider2D collision)
         {
            if (collision.CompareTag("Player"))
            {
                HeroMove.Session.Data.Coins += _goldenCoinPoint;
                Destroy(gameObject);
            }
         }
    
}
