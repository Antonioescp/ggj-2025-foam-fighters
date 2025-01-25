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
    }
}
