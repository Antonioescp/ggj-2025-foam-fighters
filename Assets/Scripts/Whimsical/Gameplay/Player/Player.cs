using UnityEngine;
using Whimsical.Gameplay.Health;

namespace Whimsical.Gameplay.Player
{
    using System;
    using Debug;
    using UnityEngine.InputSystem;
    using UnityEngine.Serialization;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private CharacterStats _stats;
        private HealthPoints _healthPoints;

        private Rigidbody2D _rigidBody;
        private SpriteRenderer _spriteRenderer;
        
        private PlayerInput _input;
        private const string JumpInput = "Jump";
        private InputAction _jumpInput;
        private const string MoveInput = "Move";
        private InputAction _moveInput;
        private const string AttackInput = "Attack";
        private InputAction _attackInput;
        private const string ParryInput = "Parry";
        private InputAction _parryInput;
        
        [SerializeField]
        private GameObject _attackHitBox;
        private BoxCollider2D _attackHitBoxCollider;
        private Timer _attackDurationTimer;
        private Timer _attackCooldownTimer;
        private bool CanAttack => !_attackDurationTimer.IsRunning
                                  && !_attackCooldownTimer.IsRunning
                                  && !_parryDurationTimer.IsRunning;
        private bool IsAttacking => _attackDurationTimer.IsRunning;
        private bool IsLookingRight => _spriteRenderer.flipX;

        private Timer _parryDurationTimer;
        private Timer _parryCooldownTimer;
        private bool IsParrying => _parryDurationTimer.IsRunning;
        private bool CanParry => !IsParrying && !IsAttacking && !_parryCooldownTimer.IsRunning;

        [Header("Audio settings")]
        [SerializeField] 
        private AudioSource _audio;
        [SerializeField]
        private AudioClip _attackClip;
        [SerializeField]
        private AudioClip _parryClip;

        [Header("Sprite settings")]
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite _attackSprite;
        [SerializeField] private Sprite _parrySprite;

        public bool IsDead => _healthPoints.CurrentHealth <= 0;

        private BoxCollider2D _collider;

        private bool IsGrounded
        {
            get
            {
                var colliderCenter = new Vector2(_collider.transform.position.x, _collider.transform.position.y);
                var hit = Physics2D.BoxCast(colliderCenter, _collider.size / 4, 0, Vector2.down);

                if (!hit)
                    return false;
                
                DebugExtensions.Log($"Hit {hit.collider.gameObject.name}");
                
                var colliderBottomCenter = new Vector2(_collider.transform.position.x, _collider.bounds.min.y);
                var maxDistanceToJump = _stats.JumpingDistance + (colliderCenter - colliderBottomCenter).magnitude;
                
                DebugExtensions.Log($"Current distance: {hit.distance}");
                DebugExtensions.Log($"Max distance: {maxDistanceToJump}");
                
                return hit.distance <= maxDistanceToJump;
            }
        }
        
        private Action<Vector2> _attackHitAction;
        public event Action<Vector2> OnAttackHit
        {
            add => _attackHitAction += value;
            remove => _attackHitAction -= value;
        }
        
        public event Action OnDeath
        {
            add => _healthPoints.OnDeath += value;
            remove => _healthPoints.OnDeath -= value;
        }

        private Action _parryAction;
        public event Action OnParry
        {
            add => _parryAction += value;
            remove => _parryAction -= value;
        }

        private Color _parryColor = Color.black;
        private Color _currentColor;

        private void Awake()
        {
            _healthPoints = new HealthPoints(_stats.BaseMaxHealth);
        }

        private void Start()
        {
            // Instantiating stuff
            DebugExtensions.Log($"My health is {_healthPoints.CurrentHealth}");
            
            // Getting stuff
            _rigidBody = this.GetComponent<Rigidbody2D>();
            _input = this.GetComponent<PlayerInput>();
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
            _attackHitBoxCollider = _attackHitBox.GetComponent<BoxCollider2D>();
            _collider = this.GetComponent<BoxCollider2D>();
            
            // Initializing input
            _jumpInput = _input.actions.FindAction(JumpInput);
            _moveInput = _input.actions.FindAction(MoveInput);
            _attackInput = _input.actions.FindAction(AttackInput);
            _parryInput = _input.actions.FindAction(ParryInput);

            // Setting timers
            _attackDurationTimer = gameObject.AddComponent<Timer>();
            _attackDurationTimer.TargetTime = _stats.AttackDuration;
            _attackDurationTimer.OnTimeElapsed += () =>
            {
                DebugExtensions.Log("Attack duration finished");
                _attackHitBoxCollider.enabled = false;
                _attackCooldownTimer.RestartTimer();

                _spriteRenderer.sprite = _idleSprite;
            };
            _attackDurationTimer.UseUnscaledDeltaTime = false;

            _attackCooldownTimer = gameObject.AddComponent<Timer>();
            _attackCooldownTimer.TargetTime = _stats.AttackCooldown;
            _attackCooldownTimer.OnTimeElapsed += () => DebugExtensions.Log($"You can attack again");
            _attackCooldownTimer.UseUnscaledDeltaTime = false;

            _currentColor = _spriteRenderer.color;
            _parryDurationTimer = gameObject.AddComponent<Timer>();
            _parryDurationTimer.TargetTime = _stats.ParryDuration;
            _parryDurationTimer.OnTimeElapsed += () =>
            {
                _parryCooldownTimer.RestartTimer();
                _spriteRenderer.sprite = _idleSprite;
            };
            _parryDurationTimer.UseUnscaledDeltaTime = false;
            
            _parryCooldownTimer = gameObject.AddComponent<Timer>();
            _parryCooldownTimer.TargetTime = _stats.ParryCooldown;
            _parryCooldownTimer.UseUnscaledDeltaTime = false;
            
            // Making sure all players can use the same keyboard
            _input.SwitchCurrentControlScheme(_input.defaultControlScheme, Keyboard.current);

            _healthPoints.OnDeath += () => DebugExtensions.Log($"{this.name} died :(");
        }

        private void Update()
        {
            if (IsLookingRight && _attackHitBoxCollider.offset.x < 0
                || !IsLookingRight && _attackHitBoxCollider.offset.x > 0)
            {
                var offset = _attackHitBoxCollider.offset;
                offset.x *= -1;
                _attackHitBoxCollider.offset = offset;
            }
        }

        private void FixedUpdate()
        {
            var movement = _moveInput.ReadValue<float>();
            _rigidBody.linearVelocityX = movement * _stats.MovementSpeed;
            
            var isMoving = movement != 0.0f;
            if (isMoving && !IsAttacking)
                _spriteRenderer.flipX = movement > 0;

            if (_jumpInput.WasPressedThisFrame() && IsGrounded)
                this.Jump();

            if (_attackInput.WasPressedThisFrame())
                this.Attack();
            
            if (_parryInput.WasPressedThisFrame())
                this.Parry();
        }

        private void Jump()
        {
            _rigidBody.AddForce(_stats.JumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        private void Attack()
        {
            if (!this.CanAttack) return;
            DebugExtensions.Log("Attacking");
            _attackHitBoxCollider.enabled = true;
            this._attackDurationTimer.RestartTimer();
            _spriteRenderer.sprite = _attackSprite;
            _audio.PlayOneShot(_attackClip);
        }

        private void Parry()
        {
            if (!this.CanParry) return;
            DebugExtensions.Log("Parrying");
            _parryDurationTimer.RestartTimer();
            _spriteRenderer.sprite = _parrySprite;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable is null)
                return;
            
            _attackHitAction?.Invoke(this._attackHitBoxCollider.transform.position);
            damageable.ReceiveDamage(1);

            // knock back
            var otherRigibBody = other.gameObject.GetComponent<Rigidbody2D>();
            var knockDirection = other.transform.position - this.transform.position;
            knockDirection.Normalize();
            
            otherRigibBody.AddForce(knockDirection * 500, ForceMode2D.Impulse);
        }

        public void ReceiveDamage(int damage)
        {
            if (_parryDurationTimer.IsRunning)
            {
                _audio.PlayOneShot(_parryClip);
                _parryAction?.Invoke();
                return;
            }
                
            
            this._healthPoints.ReceiveDamage(damage);
        }
    }
}
