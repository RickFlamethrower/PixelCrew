using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelCrew.Components
{
    public class LayerCheck : MonoBehaviour
    {
        [SerializeField] private LayerMask _layer;
        [SerializeField] private bool _isTouchingLayer;
        private Collider2D _collider;

        public bool IsTouchingLayer => _isTouchingLayer;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            _isTouchingLayer = _collider.IsTouchingLayers(_layer);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            _isTouchingLayer = _collider.IsTouchingLayers(_layer);
        }

        /*
        private void OnTriggerEnter2D (Collider2D other)
        {
        if (other.gameObject.tag.Equals ("Ground"))
        {
                Instantiate (_jumpParticle, transform.position, _jumpParticle.transform.rotation); 
        } 
        }
        */
    }
}
