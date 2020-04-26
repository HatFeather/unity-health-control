using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HatFeather.HealthControl.Editors.Tests
{
    public class TargetableTest
    {
        [Test]
        public void killTarget()
        {
            var targetable = new MockTargetable();
            targetable.setup();

            // pre conditions
            Assert.IsFalse(targetable.isDead);
            Assert.AreEqual(0, targetable.deaths);

            // do some damage
            targetable.pool.dealDamage(10);
            Assert.IsFalse(targetable.isDead);
            Assert.AreEqual(90, targetable.pool.health);
            Assert.AreEqual(100, targetable.pool.maxHealth);
            Assert.AreEqual(-10, targetable.deltaHealth);
            Assert.AreEqual(0, targetable.deltaMaxHealth);
            Assert.AreEqual(0, targetable.deaths);

            // kill the target
            targetable.pool.dealDamage(90);
            Assert.IsTrue(targetable.isDead);
            Assert.AreEqual(0, targetable.pool.health);
            Assert.AreEqual(100, targetable.pool.maxHealth);
            Assert.AreEqual(-100, targetable.deltaHealth);
            Assert.AreEqual(0, targetable.deltaMaxHealth);
            Assert.AreEqual(1, targetable.deaths);

            // can't die twice (shouldn't affect health)
            targetable.pool.dealDamage(50);
            Assert.IsTrue(targetable.isDead);
            Assert.AreEqual(0, targetable.pool.health);
            Assert.AreEqual(100, targetable.pool.maxHealth);
            Assert.AreEqual(-100, targetable.deltaHealth);
            Assert.AreEqual(0, targetable.deltaMaxHealth);
            Assert.AreEqual(1, targetable.deaths);

            targetable.tearDown();
        }

        private class MockTargetable
        {
            public HealthPool pool = new HealthPool();

            public int deaths;
            public int deltaHealth;
            public int deltaMaxHealth;

            public bool isDead => pool.isEmpty;

            public void setup()
            {
                pool.healthChanged += onHealthChanged;
                pool.maxHealthChanged += onMaxHealthChanged;
            }

            public void tearDown()
            {
                pool.healthChanged -= onHealthChanged;
                pool.maxHealthChanged -= onMaxHealthChanged;
            }

            void onHealthChanged(int prev, int curr)
            {
                if (pool.isEmpty)
                    deaths++;
                deltaHealth += curr - prev;
            }

            void onMaxHealthChanged(int prev, int curr)
            {
                deltaMaxHealth += curr - prev;
            }
        }
    }
}
