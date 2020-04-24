using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HatFeather.HealthControl
{
    public sealed class Targetable : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _maxHealth = 100;
        [SerializeField, Min(0)] private int _health = 100;
        [SerializeField] private Events _events = new Events();

        public int maxHealth
        {
            get => _maxHealth;
            set
            {
                Debug.Assert(value >= 1);

                info.previousMaxHealth = _health;
                _maxHealth = value;

                if (info.previousMaxHealth != value)
                {
                    events.onMaxHealthChanged.Invoke();
                }
            }
        }

        public int health
        {
            get => _health;
            set
            {
                Debug.Assert(value >= 0);

                info.previouslyDead = isDead;
                info.previousHealth = _health;
                _health = value;

                if (info.previousHealth != value)
                {
                    events.onHealthChanged.Invoke();
                    if (isDead && info.previousHealth != 0)
                        events.onDie.Invoke();
                }
            }
        }

        public GeneralTargetableInfo info => context.get<GeneralTargetableInfo>();
        public Context context { get; private set; } = new Context();
        public Events events => _events;
        public float percentHealth => (float)health / maxHealth;
        public bool isDead => health == 0;

        private void Awake()
        {
            var info = new GeneralTargetableInfo();

            info.previousHealth = health;
            info.previousMaxHealth = maxHealth;
            info.previouslyDead = isDead;

            context.put(info);
        }

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

        public class Context
        {
            private Dictionary<Type, object> _deps = new Dictionary<Type, object>();

            internal Context() { }

            public void put<T>(T value)
            {
                if (value == null)
                {
                    Debug.LogError("Cannot put a null value into a context");
                    return;
                }

                _deps[typeof(T)] = value;
            }

            public T get<T>()
            {
                return (T)_deps[typeof(T)];
            }

            public bool remove<T>()
            {
                return _deps.Remove(typeof(T));
            }

            public bool contains<T>()
            {
                return _deps.ContainsKey(typeof(T));
            }
        }

        [Serializable]
        public class Events
        {
            [SerializeField] private DieEvent _onDie = new DieEvent();
            [SerializeField] private MaxHealthChangedEvent _onMaxHealthChanged = new MaxHealthChangedEvent();
            [SerializeField] private HealthChangedEvent _onHealthChanged = new HealthChangedEvent();

            public DieEvent onDie => _onDie;
            public MaxHealthChangedEvent onMaxHealthChanged => _onMaxHealthChanged;
            public HealthChangedEvent onHealthChanged => _onHealthChanged;

            internal Events() { }

            [Serializable]
            public class DieEvent : UnityEvent { }

            [Serializable]
            public class MaxHealthChangedEvent : UnityEvent { }

            [Serializable]
            public class HealthChangedEvent : UnityEvent { }
        }
    }
}
