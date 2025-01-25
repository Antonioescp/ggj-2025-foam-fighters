using UnityEngine;
using Whimsical.Gameplay.Health;

namespace Whimsical.Gameplay.Player
{
    using System;
    using Debug;
    using UnityEngine.InputSystem;

    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private CharacterStats _stats;
        private HealthPoints _healthPoints;

        private const string JumpAction = "Jump";
        private InputAction _jumpAction;

        private const string MoveAction = "Move";
        private InputAction _moveAction;

        private Rigidbody2D _rb;

        private void Start()
        {
            // Instantiating stuff
            _healthPoints = new HealthPoints(_stats.BaseMaxHealth);
            
            // Getting stuff
            _rb = this.GetComponent<Rigidbody2D>();
            
            // Initializing input
            _jumpAction = InputSystem.actions.FindAction(JumpAction);
            _moveAction = InputSystem.actions.FindAction(MoveAction);
            
            DebugExtensions.Log($"My health is {_healthPoints.CurrentHealth}");
        }

        private void FixedUpdate()
        {
            var movement = _moveAction.ReadValue<Vector2>().normalized.x;
            _rb.linearVelocityX = movement * _stats.MovementSpeed;
        }
    }
}
