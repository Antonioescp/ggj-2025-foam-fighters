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
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private CharacterStats _stats;
        private HealthPoints _healthPoints;

        private Rigidbody2D _rigidBody;
        private SpriteRenderer _spriteRenderer;
        
        private PlayerInput _input;
        private const string JumpAction = "Jump";
        private InputAction _jumpAction;

        private const string MoveAction = "Move";
        private InputAction _moveAction;

        private bool IsGrounded
        {
            get
            {
                var spriteCenter = new Vector2(_spriteRenderer.transform.position.x, _spriteRenderer.transform.position.y);
                var hit = Physics2D.BoxCast(spriteCenter, _spriteRenderer.size / 2, 0, Vector2.down);

                if (!hit)
                    return false;
                
                DebugExtensions.Log($"Hit against {hit.collider.gameObject.name}");
                
                
                var spriteBottomCenter = new Vector2(_spriteRenderer.transform.position.x, _spriteRenderer.bounds.min.y);

                var minDistanceToJump = _stats.JumpingDistance + (spriteCenter - spriteBottomCenter).magnitude;
                
                DebugExtensions.Log($"The distance to jump is {minDistanceToJump}");
                DebugExtensions.Log($"The distance is {hit.distance}");
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
            
            // Initializing input
            _jumpAction = _input.actions.FindAction(JumpAction);
            _moveAction = _input.actions.FindAction(MoveAction);
            
            _input.SwitchCurrentControlScheme(_input.defaultControlScheme, Keyboard.current);
        }

        private void FixedUpdate()
        {
            var movement = _moveAction.ReadValue<float>();
            _rigidBody.linearVelocityX = movement * _stats.MovementSpeed;

            if (_jumpAction.WasPressedThisFrame() && IsGrounded)
            {
                this.Jump();
            }
        }

        private void Jump()
        {
            _rigidBody.AddForce(_stats.JumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
}
