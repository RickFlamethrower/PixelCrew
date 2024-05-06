using System.Collections;
using System.Collections.Generic;
using PixelCrew;
using PixelCrew.Components;
using PixelCrew.Model;
using PixelCrew.Utils;
using UnityEditor.Animations;
using UnityEngine;

public class HeroMove : MonoBehaviour
{
    
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _damagejumpSpeed;
    [SerializeField] private float _fallVelocity;
    [SerializeField] private int damage;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerCheck _groundCheck;

    [SerializeField] private AnimatorController _armed;
    [SerializeField] private AnimatorController _disarmed;


    [SerializeField] private CheckCircleOverlap _attackRange;

    [Space] [Header("Particles")]
    [SerializeField] private SpawnComponent _footParticles;
    [SerializeField] private SpawnComponent _jumpParticles;
    [SerializeField] private SpawnComponent _fallParticles;
    [SerializeField] private ParticleSystem _hitPatricles;
    
    private Collider2D[] _interactionResult = new Collider2D[1];
    private Rigidbody2D _rigidbody;
    private Vector2 _direction;
    private Animator _animator;
    private bool _isGrounded;
    private bool _allowDoubleJump;
    private bool _isJumping;

    private static readonly int IsGroundKey = Animator.StringToHash("is-ground");
    private static readonly int IsRunning = Animator.StringToHash("is-running");
    private static readonly int VerticalVelosity = Animator.StringToHash("vertical-velocity");
    private static readonly int Hit = Animator.StringToHash("hit");
    private static readonly int AttackKey = Animator.StringToHash("attack");

    private GameSession _session;

    private void Awake()
    {
          _rigidbody = GetComponent<Rigidbody2D>();
          _animator = GetComponent<Animator>();
          //_sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _session = FindObjectOfType<GameSession>();
        
        UpdateHeroWeapon();
    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    private void Update()
    {
        _isGrounded = IsGrounded();
    }

    private void FixedUpdate()
    {
        var xVelocity = _direction.x * _speed; //координату Х расчитываем исходя из направления движения
        var yVelocity = CalculateYVelosity(); // координата У...
        _rigidbody.velocity = new Vector2(xVelocity, yVelocity);

        _animator.SetBool("is-ground", _isGrounded);
        _animator.SetBool("is-running", _direction.x != 0);
        _animator.SetFloat("vertical-velocity", _rigidbody.velocity.y);

        UpdateSpriteDirection();
    }

    private float CalculateYVelosity()  //...координата У
    {
        var yVelocity = _rigidbody.velocity.y;  
        var isJumpingPressing = _direction.y > 0;
        
        if(_isGrounded) 
        {
            _allowDoubleJump = true; // при косании земли двойной прыжек снова возможен.
            _isJumping = false;
        }
        if (isJumpingPressing)
        {   
            _isJumping = true;
            yVelocity = CalculateJumpVelocity(yVelocity); 
        }
        else if (_rigidbody.velocity.y > 0 && _isJumping)
        {
            yVelocity *= 0.5f;
        }
        return yVelocity;
    }

    private float CalculateJumpVelocity(float yVelocity)
    {
        var isFalling = _rigidbody.velocity.y <= 0.001f; //если тело пдает
        if (!isFalling) return yVelocity; // если не падает, возвращает дефолтное значение

        if (_isGrounded)  // если мы на замле, то происходит прыжок
        {
            yVelocity += _jumpSpeed;
            _jumpParticles.Spawn();
            
        }
         else if (_allowDoubleJump) // иначе, если доступен двойной прыжек, то - двойной прыжок
        {
            yVelocity = _jumpSpeed;
            _allowDoubleJump = false; // следующий двойной прыжет запрещаем.
            
        }
        return yVelocity;
    }

    private void UpdateSpriteDirection()
    {
        if (_direction.x > 0)
        {
            transform.localScale = Vector3.one; //константа - эквивалентен единичному вектору
            //_sprite.flipX = false;
        } 
        else if (_direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //_sprite.flipX = true;
        }
    }

    private bool IsGrounded()
    {
        return _groundCheck.IsTouchingLayer;
        
        //   var hit = Physics2D.CircleCast(transform.position + _groundCheckPositionDelta, _groundCheckRadius, Vector2.down, 0, _groundLayer);

        //   return hit.collider != null;

          // var hit = Physics2D.Raycast(transform.position, Vector2.down, 1, _groundLayer);
          // return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
          // Debug.DrawRay(transform.position, Vector3.down, IsGrounded() ? Color.green : Color.red);
          Gizmos.color = IsGrounded() ? Color.green : Color.red;
          Gizmos.DrawSphere(transform.position, 0.3f);
    }

    public void TakeDamage()
    {
        _isJumping = false;
        _animator.SetTrigger(Hit);
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damagejumpSpeed);

        if(_session.Data.Coins > 0)
        {
            SpawnCoins();
        }
        
    }
    private void SpawnCoins()
    {
        var numCoinsToDispose = Mathf.Min(_session.Data.Coins, 5); //сколько можем выкинуть 5 или меньше
        _session.Data.Coins -= numCoinsToDispose; // отнимаем из общего количества монет

        var burst = _hitPatricles.emission.GetBurst(0); // в партикл системе устанавливем количество выбрасываемых монет
        burst.count = numCoinsToDispose;
        _hitPatricles.emission.SetBurst(0, burst); // сохраняем в емишине

        _hitPatricles.gameObject.SetActive(true); // активируем партикл систему
        _hitPatricles.Play(); // применяем ее
    }
    
    public void Interact()
    {
        var size = Physics2D.OverlapCircleNonAlloc(
            transform.position, 
            _interactionRadius, 
            _interactionResult, 
            _interactionLayer);

        for (int i = 0; i < size; i++)
        {
            var interactable = _interactionResult[i].GetComponent<InteractableComponent>();
            if(interactable != null)
            {
                interactable.Interact();
            }
        }
    } 

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.IsInLayer(_groundLayer))
        {
            var contact = other.contacts[0];
            if (contact.relativeVelocity.y >= _fallVelocity)
            {
                _fallParticles.Spawn();
            }
        }
    }

    public void SpawnFootDust()
    {
        _footParticles.Spawn();
    }

    public void Attack()
    {
        if (!_session.Data.IsArmed) return;
        
        _animator.SetTrigger(AttackKey);
    }

    public void OnAttack()
    {
         var gos = _attackRange.GetObjectsInRange();
        foreach (var go in gos)
        {
            var hp = go.GetComponent<HealthComponent>();
            if (hp != null && go.CompareTag("Enemy"))
            {
                hp.ApplyDamage(damage);  
            }
        }
    }

    public void ArmHero()
    {
        _session.Data.IsArmed = true;
        _animator.runtimeAnimatorController = _armed;
        UpdateHeroWeapon();
    }

    private void UpdateHeroWeapon()
    {
         _animator.runtimeAnimatorController = _session.Data.IsArmed ? _armed : _disarmed;
    }
    
}
