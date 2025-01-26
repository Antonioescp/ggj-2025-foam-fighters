namespace Whimsical.Gameplay.Health
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/CharacterStats")]
    public class CharacterStats : ScriptableObject
    {
        [Header("Health settings")]
        [field: SerializeField]
        public int BaseMaxHealth { get; set; }
        
        [Header("Movement settings")]
        [field: SerializeField]
        public float MovementSpeed { get; set; }
        
        [field: SerializeField]
        public float JumpForce { get; set; }
        
        [field: SerializeField]
        public float JumpingDistance { get; set; }
        
        [Header("Balancing settings")]
        [field: SerializeField]
        public float AttackDuration { get; set; }
        
        [field: SerializeField]
        public float AttackCooldown { get; set; }
        
        [field: SerializeField]
        public float ParryDuration { get; set; }
        
        [field: SerializeField]
        public float ParryCooldown { get; set; }
    }
}
