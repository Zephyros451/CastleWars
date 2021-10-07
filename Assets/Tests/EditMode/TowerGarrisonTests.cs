using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class TowerGarrisonTests
    {
        public class DecreaseGarrisonCount
        {
            [Test]
            public void CountValueDecreases_When_PositiveArgumentPassed()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower, 1f);

                float beforeDecrease = garrison.Count;
                garrison.DecreaseGarrisonCount(5);
                float afterDecrease = garrison.Count;

                Assert.Greater(beforeDecrease, afterDecrease);
            }

            [Test]
            public void CountValueDecreases_When_NegativeArgumentPassed()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower, 1f);

                float beforeDecrease = garrison.Count;
                garrison.DecreaseGarrisonCount(-5);
                float afterDecrease = garrison.Count;

                Assert.Greater(beforeDecrease, afterDecrease);
            }

            [Test]
            public void CountValueDoesntChange_When_ZeroPassed()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower, 1f);

                float beforeDecrease = garrison.Count;
                garrison.DecreaseGarrisonCount(0);
                float afterDecrease = garrison.Count;

                Assert.AreEqual(beforeDecrease, afterDecrease);
            }
        }

        public class SetCount
        {
            [Test]
            public void CountValueSetsToZero_When_NegativeValuePassed()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower, 1f);

                garrison.Count = -5;

                Assert.AreEqual(0, garrison.Count);
            }

            [Test]
            public void CountValueSetsWithoutModifications()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower, 1f);

                garrison.Count = 123456;

                Assert.AreEqual(123456, garrison.Count);
            }
        }
    }
}
