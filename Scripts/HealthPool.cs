using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HatFeather.HealthControl
{
    [Serializable]
    public sealed class HealthPool
    {
        public const int MaxHealthLowerBound = 1;
        public const int HealthLowerBound = 0;

        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _health = 100;

        public event HealthChangeCallback maxHealthChanged = null;
        public event HealthChangeCallback healthChanged = null;

        public delegate void HealthChangeCallback(int previousHealth, int currentHealth);

        public int maxHealth
        {
            get => _maxHealth;
            set
            {
                Debug.Assert(value >= MaxHealthLowerBound);

                int prev = _health;
                _maxHealth = value;

                if (prev != value && maxHealthChanged != null)
                    maxHealthChanged(prev, value);
            }
        }

        public int health
        {
            get => _health;
            set
            {
                Debug.Assert(value >= HealthLowerBound);

                int prev = _health;
                _health = value;

                if (prev != value && healthChanged != null)
                    healthChanged(prev, value);
            }
        }

        public float percentHealth => (float)health / maxHealth;
        public bool isEmpty => health == HealthLowerBound;

        public void dealDamage(int amount)
        {
            Debug.Assert(amount >= 0);

            health = Mathf.Max(0, health - amount);
        }

        public void restoreHealth(int amount, bool capHealth = true)
        {
            Debug.Assert(amount >= 0);

            if (capHealth)
                health = Mathf.Min(maxHealth, health + amount);
            else
                health += amount;
        }
    }
}
