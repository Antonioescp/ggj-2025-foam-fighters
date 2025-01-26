namespace Whimsical.Gameplay.Health
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [field: SerializeField]
        public int BaseMaxHealth { get; set; }
        
        [field: SerializeField]
        public float MovementSpeed { get; set; }
        
        [field: SerializeField]
        public float JumpForce { get; set; }
        
        [field: SerializeField]
        public float JumpingDistance { get; set; }
        
        [field: SerializeField]
        public float AttackDuration { get; set; }
        
        [field: SerializeField]
        public float AttackCooldown { get; set; }
    }
}
