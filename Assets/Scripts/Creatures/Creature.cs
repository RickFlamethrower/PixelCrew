using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using PixelCrew.Components;
using UnityEngine;

namespace PixelCrew.Creatures
{
    public class Creature : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private float _speed;
        [SerializeField] protected float _jumpSpeed;
        [SerializeField] private float _damageVelocity;
        [SerializeField] private int damage;

        [Header ("Chekers")]
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private CheckCircleOverlap _attackRange;
        [SerializeField] protected SpawnListComponents _particles;

        protected Rigidbody2D Rigidbody;
        protected Vector2 Direction;
        protected Animator Animator;
        protected bool IsGrounded;
        private bool _isJumping;

        private static readonly int IsGroundKey = Animator.StringToHash("is-ground");
        private static readonly int IsRunning = Animator.StringToHash("is-running");
        private static readonly int VerticalVelosity = Animator.StringToHash("vertical-velocity");
        private static readonly int Hit = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        protected virtual void Update()
        {
            IsGrounded = _groundCheck.IsTouchingLayer;
        }

        private void FixedUpdate()
        {
            var xVelocity = Direction.x * _speed; //координату Х расчитываем исходя из направления движения
            var yVelocity = CalculateYVelosity(); // координата У...
            Rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            Animator.SetBool("is-ground", IsGrounded);
            Animator.SetBool("is-running", Direction.x != 0);
            Animator.SetFloat("vertical-velocity", Rigidbody.velocity.y);

            UpdateSpriteDirection();
        }

        protected virtual float CalculateYVelosity()  //...координата У
        {
            var yVelocity = Rigidbody.velocity.y;  
            var isJumpingPressing = Direction.y > 0;
            
            if(IsGrounded) 
            {
                _isJumping = false;
            }
            if (isJumpingPressing)
            {   
                _isJumping = true;

                var isFalling = Rigidbody.velocity.y <= 0.001f; //если тело пдает
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity; 
            }
            else if (Rigidbody.velocity.y > 0 && _isJumping)
            {
                yVelocity *= 0.5f;
            }
            return yVelocity;
        }

        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded)  // если мы на замле, то происходит прыжок
            {
                yVelocity = _jumpSpeed;
                _particles.Spawn("Jump");
            }
            
            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            if (Direction.x > 0)
            {
                transform.localScale = Vector3.one; //константа - эквивалентен единичному вектору
                //_sprite.flipX = false;
            } 
            else if (Direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                //_sprite.flipX = true;
            }
        }

        public virtual void TakeDamage()
        {
            _isJumping = false;
            Animator.SetTrigger(Hit);
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, _damageVelocity);
        }

        public virtual void Attack()
        {
            Animator.SetTrigger(AttackKey);
        }

        public void OnDoAttack()
        {
            var gos = _attackRange.GetObjectsInRange();
            
            foreach (var go in gos)
            {
                var hp = go.GetComponent<HealthComponent>();
                if (hp != null && go.CompareTag("Enemy"))
                {
                    hp.ModifyHealth(damage);  
                }
            }
        }

    }      
}
