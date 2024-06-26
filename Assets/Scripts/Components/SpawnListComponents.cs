using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Components
{
    public class SpawnListComponents : MonoBehaviour
    {
        [SerializeField] public SpawnData[] _spawners;

        public void Spawn(string id)
        {
            var spawner = _spawners.FirstOrDefault(element => element.Id == id);
            spawner?.Component.Spawn(); 
        } 

       [Serializable]
       public class SpawnData
       {
            public string Id;
            public SpawnComponent Component;
       } 
    }
}
