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

        [Test]
        public void injectContextInfo()
        {
            var targetable = new MockTargetable();
            targetable.setup();

            // no mock info should exist
            Assert.IsFalse(targetable.context.contains<MockInfo>());
            Assert.IsNull(targetable.context.get<MockInfo>());

            var infoToInject = new MockInfo();
            targetable.context.put(infoToInject);

            // mock info should be injected
            Assert.IsTrue(targetable.context.contains<MockInfo>());
            Assert.IsNotNull(targetable.context.get<MockInfo>());

            targetable.context.get<MockInfo>().attackerName = "test";
            targetable.context.get<MockInfo>().attackerPosition = Vector3.one;
            targetable.context.get<MockInfo>().prevHealth = targetable.pool.health;
            targetable.context.get<MockInfo>().prevMaxHealth = targetable.pool.maxHealth;
            targetable.pool.health -= 50;
            targetable.pool.maxHealth += 50;

            // the info changes should take effect
            Assert.AreEqual(infoToInject, targetable.context.get<MockInfo>());
            Assert.AreEqual("test", targetable.context.get<MockInfo>().attackerName);
            Assert.AreEqual(Vector3.one, targetable.context.get<MockInfo>().attackerPosition);
            Assert.AreEqual(100, targetable.context.get<MockInfo>().prevHealth);
            Assert.AreEqual(100, targetable.context.get<MockInfo>().prevMaxHealth);
            Assert.AreEqual(50, targetable.pool.health);
            Assert.AreEqual(150, targetable.pool.maxHealth);

            targetable.tearDown();
        }

        private class MockTargetable
        {
            public HealthPool pool = new HealthPool();
            public HealthContext context = new HealthContext();

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

        private class MockInfo : IHealthContextInjectable
        {
            public int prevHealth { get; set; } = 0;
            public int prevMaxHealth { get; set; } = 0;
            public Vector3? attackerPosition { get; set; } = null;
            public string attackerName { get; set; } = null;
        }
    }
}
