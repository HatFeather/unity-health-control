using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HatFeather.HealthControl.Editors.Tests
{
    public class TargetableTest
    {
        [UnityTest]
        public IEnumerator killTarget()
        {
            var sim = new Sim();
            yield return sim.setup();

            sim.targetable.maxHealth = 100;
            sim.targetable.health = 100;

            // make sure pre conditions are met
            Assert.AreEqual(0, sim.deathCount);
            Assert.True(sim.totalHealthChange == 0);
            Assert.AreEqual(sim.targetable.health, sim.targetable.info.previousHealth);
            Assert.AreEqual(sim.targetable.maxHealth, sim.targetable.info.previousMaxHealth);
            Assert.AreEqual(sim.targetable.isDead, sim.targetable.info.previouslyDead);

            // 70 health
            sim.targetable.dealDamage(30);
            Assert.AreEqual(-30, sim.totalHealthChange);
            Assert.AreEqual(70, sim.targetable.health);
            Assert.IsFalse(sim.targetable.info.previouslyDead);
            Assert.IsFalse(sim.targetable.isDead);

            // 80 health
            sim.targetable.restoreHealth(10);
            Assert.AreEqual(-20, sim.totalHealthChange);
            Assert.AreEqual(80, sim.targetable.health);
            Assert.IsFalse(sim.targetable.info.previouslyDead);
            Assert.IsFalse(sim.targetable.isDead);

            // -10 = 0 health = dead
            sim.targetable.dealDamage(90);
            Assert.AreEqual(-100, sim.totalHealthChange);
            Assert.AreEqual(0, sim.targetable.health);
            Assert.AreEqual(1, sim.deathCount);
            Assert.AreEqual(80, sim.targetable.info.previousHealth);
            Assert.True(sim.targetable.isDead);
            Assert.IsFalse(sim.targetable.info.previouslyDead);

            // -90 = 0 health = dead (should only die once if already dead)
            sim.targetable.dealDamage(90);
            Assert.AreEqual(-100, sim.totalHealthChange);
            Assert.AreEqual(0, sim.targetable.health);
            Assert.AreEqual(1, sim.deathCount);
            Assert.True(sim.targetable.isDead);
            Assert.True(sim.targetable.info.previouslyDead);

            yield return sim.teardown();
        }

        [UnityTest]
        public IEnumerator injectContextInfo()
        {
            var sim = new Sim();
            yield return sim.setup();

            var targetable = sim.targetable;
            targetable.maxHealth = 100;
            targetable.health = 100;

            // check pre conditions
            Assert.False(targetable.context.contains<MockInfo>());
            Assert.Null(targetable.context.get<MockInfo>());

            // inject mock info
            var mockInfo = new MockInfo();
            sim.targetable.context.put(mockInfo);

            // check post conditions
            Assert.True(targetable.context.contains<MockInfo>());
            Assert.NotNull(targetable.context.get<MockInfo>());
            Assert.IsNull(targetable.context.get<MockInfo>().attackerName);
            Assert.IsNull(targetable.context.get<MockInfo>().attackerPosition);

            // simulate some info changes
            targetable.context.get<MockInfo>().attackerName = "Batman";
            targetable.context.get<MockInfo>().attackerPosition = Vector3.one;
            targetable.dealDamage(30);

            // verify changes took effect
            Assert.AreEqual(targetable.context.get<MockInfo>().attackerName, "Batman");
            Assert.AreEqual(targetable.context.get<MockInfo>().attackerPosition, Vector3.one);
            Assert.AreEqual(70, targetable.health);

            yield return sim.teardown();
        }

        private class MockInfo
        {
            public Vector3? attackerPosition { get; set; } = null;
            public string attackerName { get; set; } = null;
        }

        private class Sim
        {
            public Targetable targetable;

            public int totalHealthChange = 0;
            public int healthChangeCount = 0;
            public int maxHealthChangeCount = 0;
            public int deathCount = 0;

            public IEnumerator setup()
            {
                targetable = new GameObject().AddComponent<Targetable>();
                yield return null;

                targetable.events.onDie.AddListener(onDie);
                targetable.events.onHealthChanged.AddListener(onHealthChanged);
                targetable.events.onMaxHealthChanged.AddListener(onMaxHealthChanged);
                targetable.restoreDefaultContext();
            }

            public IEnumerator teardown()
            {
                GameObject.DestroyImmediate(targetable.gameObject);
                yield return null;
            }

            private void onDie()
            {
                deathCount++;
            }

            private void onHealthChanged()
            {
                totalHealthChange += targetable.health - targetable.info.previousHealth;
                healthChangeCount++;
            }

            private void onMaxHealthChanged()
            {
                maxHealthChangeCount++;
            }
        }
    }
}
