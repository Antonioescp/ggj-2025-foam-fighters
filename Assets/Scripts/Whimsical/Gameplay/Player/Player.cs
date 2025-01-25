using UnityEngine;
using Whimsical.Gameplay.Health;

namespace Whimsical.Gameplay.Player
{
    using System;
    using Debug;
    using UnityEngine.InputSystem;

    public class Player : MonoBehaviour
    {
        [SerializeField]
        private CharacterStats _stats;
        private HealthPoints _healthPoints;

        private const string JumpAction = "Jump";
        private InputAction _jumpAction;

        private const string MoveAction = "Move";
        private InputAction _moveAction;

        private void Start()
        {
            // Instantiating stuff
            _healthPoints = new HealthPoints(_stats.BaseMaxHealth);
            
            // Initializing input
            _jumpAction = InputSystem.actions.FindAction(JumpAction);
            _moveAction = InputSystem.actions.FindAction(MoveAction);
            
            DebugExtensions.Log($"My health is {_healthPoints.CurrentHealth}");
        }

        private void Update()
        {
            var movement = _moveAction.ReadValue<Vector2>();
            if (_jumpAction.IsPressed())
            {
                DebugExtensions.Log("I'm trying to jump");
            }
        }
    }
}
