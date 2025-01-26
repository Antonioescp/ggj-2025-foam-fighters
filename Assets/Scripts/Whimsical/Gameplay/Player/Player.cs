using UnityEngine;
using Whimsical.Gameplay.Health;

namespace Whimsical.Gameplay.Player
{
    using System;
    using Debug;
    using UnityEngine.InputSystem;
    using Debug = UnityEngine.Debug;

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
        
        [SerializeField]
        private GameObject _attackHitBox;
        private BoxCollider2D _attackHitBoxCollider;
        private Timer _attackDurationTimer;
        private Timer _attackCooldownTimer;
        private bool CanAttack => !_attackDurationTimer.IsRunning && !_attackCooldownTimer.IsRunning;
        private bool IsAttacking => _attackDurationTimer.IsRunning;
        private bool IsLookingRight => !_spriteRenderer.flipX;
        
        private Action<Vector2> _attackHitAction;
        public event Action<Vector2> OnAttackHit
        {
            add => _attackHitAction += value;
            remove => _attackHitAction -= value;
        }

        private bool IsGrounded
        {
            get
            {
                var spriteCenter = new Vector2(_spriteRenderer.transform.position.x, _spriteRenderer.transform.position.y);
                var hit = Physics2D.BoxCast(spriteCenter, _spriteRenderer.size / 2, 0, Vector2.down);

                if (!hit)
                    return false;
                
                var spriteBottomCenter = new Vector2(_spriteRenderer.transform.position.x, _spriteRenderer.bounds.min.y);

                var minDistanceToJump = _stats.JumpingDistance + (spriteCenter - spriteBottomCenter).magnitude;
                return hit.distance <= minDistanceToJump;
            }
        }

        private void Start()
        {
            // Instantiating stuff
            _healthPoints = new HealthPoints(_stats.BaseMaxHealth);
            DebugExtensions.Log($"My health is {_healthPoints.CurrentHealth}");
            
            // Getting stuff
            _rigidBody = this.GetComponent<Rigidbody2D>();
            _input = this.GetComponent<PlayerInput>();
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
            _attackHitBoxCollider = _attackHitBox.GetComponent<BoxCollider2D>();
            
            // Initializing input
            _jumpInput = _input.actions.FindAction(JumpInput);
            _moveInput = _input.actions.FindAction(MoveInput);
            _attackInput = _input.actions.FindAction(AttackInput);

            // Setting timers
            _attackDurationTimer = gameObject.AddComponent<Timer>();
            _attackDurationTimer.TargetTime = _stats.AttackDuration;
            _attackDurationTimer.OnTimeElapsed += () =>
            {
                DebugExtensions.Log("Attack duration finished");
                _attackHitBoxCollider.enabled = false;
                _attackCooldownTimer.RestartTimer();
            };

            _attackCooldownTimer = gameObject.AddComponent<Timer>();
            _attackCooldownTimer.TargetTime = _stats.AttackCooldown;
            _attackCooldownTimer.OnTimeElapsed += () => DebugExtensions.Log($"You can attack again");
            
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
                _spriteRenderer.flipX = movement < 0;

            if (_jumpInput.WasPressedThisFrame() && IsGrounded)
                this.Jump();

            if (_attackInput.WasPressedThisFrame())
                this.Attack();
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
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable is null)
                return;
            
            _attackHitAction?.Invoke(this._attackHitBoxCollider.transform.position);
            damageable.ReceiveDamage(1);
        }

        public void ReceiveDamage(int damage)
        {
            this._healthPoints.ReceiveDamage(damage);
        }
    }
}
