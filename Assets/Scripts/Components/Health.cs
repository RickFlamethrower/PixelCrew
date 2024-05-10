using System.Collections;
using System.Collections.Generic;
using PixelCrew.Components;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public Text healthText;
    
    public void HealthCount()
    {
        var HealthComponent = GetComponent<HealthComponent>();
        healthText.text = "Health:" + HealthComponent._health;
    }
    
    public void Update()
    {
        HealthCount();
    }
}
