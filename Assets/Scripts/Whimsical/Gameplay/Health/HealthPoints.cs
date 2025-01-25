namespace Whimsical.Gameplay.Health
{
    using System;
    using UnityEngine;

    public class HealthPoints : IDamageable
    {
        public int CurrentHealth { get; private set; }

        public int MaxHealth { get; private set; }

        private Action _onDeathAction;
        public event Action OnDeath
        {
            add => _onDeathAction += value;
            remove => _onDeathAction -= value;
        } 

        public HealthPoints(int maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public HealthPoints(int maxHealth, int currentHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
        }

        public void ReceiveDamage(int damage)
        {
            if (damage < 0)
                throw new InvalidOperationException($"The damage shouldn't be negative, value: {damage}");
            else if (damage == 0)
                Debug.LogWarning($"The damage is zero for some reason, value: {damage}");

            var resultingHealth = CurrentHealth - damage;
            var previousHealth = CurrentHealth;
            CurrentHealth = Math.Clamp(resultingHealth, 0, CurrentHealth);

            if (previousHealth > 0 && CurrentHealth <= 0)
            {
                _onDeathAction.Invoke();
            }
        }

        public void Heal(int healthPoints)
        {
            if (healthPoints < 0)
                throw new InvalidOperationException($"The healing shouldn't be negative, value: {healthPoints}");
            else if (healthPoints == 0)
                Debug.LogWarning($"The healing is zero for some reason, value: {healthPoints}");

            var resultingHealth = CurrentHealth + healthPoints;
            CurrentHealth = Math.Clamp(resultingHealth, CurrentHealth, MaxHealth);
        }

        public void IncreaseMaxHealth(int healthPoints)
        {
            if (healthPoints < 0)
                throw new InvalidOperationException($"The max health increase shouldn't be negative, value: {healthPoints}");
            else if (healthPoints == 0)
                Debug.LogWarning($"The max health increase is zero for some reason, value: {healthPoints}");

            MaxHealth += healthPoints;
        }
    }
}
