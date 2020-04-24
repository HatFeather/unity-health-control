using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HatFeather.HealthControl
{
    public class HealthContext
    {
        private Dictionary<Type, IHealthContextInjectable> _deps = new Dictionary<Type, IHealthContextInjectable>();

        public HealthContext() { }

        public void clear()
        {
            _deps.Clear();
        }

        public void put<T>(T value) where T : IHealthContextInjectable
        {
            Debug.Assert(value != null);

            _deps[typeof(T)] = value;
        }

        public T get<T>() where T : IHealthContextInjectable
        {
            var type = typeof(T);
            if (_deps.ContainsKey(type))
                return (T)_deps[type];
            return default(T);
        }

        public bool remove<T>() where T : IHealthContextInjectable
        {
            return _deps.Remove(typeof(T));
        }

        public bool contains<T>() where T : IHealthContextInjectable
        {
            return _deps.ContainsKey(typeof(T));
        }
    }
}
