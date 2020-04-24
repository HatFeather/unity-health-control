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
            var mock = new Mock();
            yield return mock.setup();

            mock.targetable.maxHealth = 100;
            mock.targetable.health = 100;

            Assert.AreEqual(0, mock.deathCount);
            Assert.IsTrue(mock.totalHealthChange == 0);
            Assert.AreEqual(0, mock.deathCount);

            // 70 health
            mock.targetable.dealDamage(30);
            Assert.AreEqual(-30, mock.totalHealthChange);
            Assert.AreEqual(70, mock.targetable.health);

            // 80 health
            mock.targetable.restoreHealth(10);
            Assert.AreEqual(-20, mock.totalHealthChange);
            Assert.AreEqual(80, mock.targetable.health);

            // -10 = 0 health = dead
            mock.targetable.dealDamage(90);
            Assert.AreEqual(-100, mock.totalHealthChange);
            Assert.AreEqual(0, mock.targetable.health);
            Assert.AreEqual(1, mock.deathCount);
            Assert.IsTrue(mock.targetable.isDead);

            yield return mock.teardown();
        }

        private class Mock
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
                totalHealthChange += delta.curr - delta.prev;
                healthChangeCount++;
            }

            private void onMaxHealthChanged()
            {
                maxHealthChangeCount++;
            }
        }
    }
}
