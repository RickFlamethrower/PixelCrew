using System.Collections;
using System.Collections.Generic;
using PixelCrew;
using PixelCrew.Components;
using PixelCrew.Creatures;
using PixelCrew.Model;
using PixelCrew.Utils;
using UnityEditor.Animations;
using UnityEngine;

public class HeroMove : Creature
{
    [SerializeField] private float _fallVelocity;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactionLayer;

    [SerializeField] private AnimatorController _armed;
    [SerializeField] private AnimatorController _disarmed;

    [Space] [Header("Particles")]
    [SerializeField] private ParticleSystem _hitPatricles;
    
    private readonly Collider2D[] _interactionResult = new Collider2D[1];
    private bool _allowDoubleJump;
    private bool _isOnWall;
    private static GameSession _session;

    protected override void Awake()
    {
        base.Awake();
        
    }

    public void OnHealthChanged(int currentHealth)
    {
        _session.Data.Hp = currentHealth;
    }

    private void Start()
    {
        _session = FindObjectOfType<GameSession>();
        var health = GetComponent<HealthComponent>();

        health.SetHealth(_session.Data.Hp);
        UpdateHeroWeapon();
    }

    public static GameSession Session
    {
        get { return _session; }
        // set { _session = value; } // Раскомментируйте, если нужен сеттер
    }

    

    protected override void Update()
    {
        base.Update();
    }

    

    protected override float CalculateYVelosity()  //...координата У
    {
        var isJumpingPressing = Direction.y > 0;
        
        if(IsGrounded || _isOnWall) 
        {
            _allowDoubleJump = true; // при косании земли двойной прыжек снова возможен.
        }
        if (!isJumpingPressing && _isOnWall)
        {   
            return 0f; 
        }
        
        return base.CalculateYVelosity();
    }

    protected override float CalculateJumpVelocity(float yVelocity)
    {
        if (!IsGrounded && _allowDoubleJump)
        {
            _particles.Spawn("Jump");//jump
            _allowDoubleJump = false; // следующий двойной прыжет запрещаем.
            return _jumpSpeed;
        }
        return base.CalculateJumpVelocity(yVelocity);
    }

    public override void TakeDamage()
    {
        base.TakeDamage();
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
                _particles.Spawn("Fall");//Fall
            }
        }
    }

    public override void Attack()
    {
        if (!_session.Data.IsArmed) return;

        base.Attack();
    }

    public void ArmHero()
    {
        _session.Data.IsArmed = true;
        Animator.runtimeAnimatorController = _armed;
        UpdateHeroWeapon();
    }

    private void UpdateHeroWeapon()
    {
         Animator.runtimeAnimatorController = _session.Data.IsArmed ? _armed : _disarmed;
    }
    
}
