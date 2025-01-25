using UnityEngine;
using Whimsical.Gameplay.Health;

namespace Whimsical.Gameplay.Player
{
    using System;
    using Debug;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private CharacterStats _stats;
        private HealthPoints _healthPoints;

        private Rigidbody2D _rb;
        
        private PlayerInput _input;
        private const string JumpAction = "Jump";
        private InputAction _jumpAction;

        private const string MoveAction = "Move";
        private InputAction _moveAction;

        private void Start()
        {
            // Instantiating stuff
            _healthPoints = new HealthPoints(_stats.BaseMaxHealth);
            DebugExtensions.Log($"My health is {_healthPoints.CurrentHealth}");
            
            // Getting stuff
            _rb = this.GetComponent<Rigidbody2D>();
            _input = this.GetComponent<PlayerInput>();
            
            // Initializing input
            _jumpAction = _input.actions.FindAction(JumpAction);
            _moveAction = _input.actions.FindAction(MoveAction);
            
            _input.SwitchCurrentControlScheme(_input.defaultControlScheme, Keyboard.current);
        }

        private void FixedUpdate()
        {
            var movement = _moveAction.ReadValue<float>();
            _rb.linearVelocityX = movement * _stats.MovementSpeed;
        }
    }
}
