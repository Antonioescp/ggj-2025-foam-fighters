using UnityEngine;
using Whimsical.Gameplay.Health;

namespace Whimsical.Gameplay.Player
{
    using Debug;

    public class Player : MonoBehaviour
    {
        [SerializeField]
        private CharacterStats _stats;
        private HealthPoints _healthPoints;

        private void Start()
        {
            _healthPoints = new HealthPoints(_stats.BaseMaxHealth);
            
            DebugExtensions.Log($"Our health is {_healthPoints.CurrentHealth}");
        }
    }
}
