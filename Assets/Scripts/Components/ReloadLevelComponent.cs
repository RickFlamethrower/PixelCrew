using System.Collections;
using System.Collections.Generic;
using PixelCrew.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrew.Components
{
    public class ReloadLevelComponent : MonoBehaviour
    {
        public void Reload()
        {
            var session = FindObjectOfType<GameSession>();
            Destroy(session);
            
            
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
            //HeroMove.Session.Data.Coins = 5;
        }   
    }
}

