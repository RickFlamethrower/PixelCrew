using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Model
{
    public class GameSession : MonoBehaviour
    {
        [SerializeField] private PlayerData _data;
        public PlayerData Data => _data;

        private void Awake()
        {
            if(IsSessionExit()) //если сессия существует, найденная сессия появилась второй - ее нужно уничтожить
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(this);//создает хранилише которое не уничтожается между переходами сцен
            }
        }

        private bool IsSessionExit()
        {
            var sessions = FindObjectsOfType<GameSession>();
            foreach (var gameSession in sessions)
            {
                if (gameSession != this) 
                    return true;
            }
                return false;
        }
    }
}

