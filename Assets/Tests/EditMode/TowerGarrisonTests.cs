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
                TowerGarrison garrison = new TowerGarrison(tower);

                float beforeDecrease = garrison.Count;
                garrison.PopFromGarrison(5);
                float afterDecrease = garrison.Count;

                Assert.Greater(beforeDecrease, afterDecrease);
            }

            [Test]
            public void CountValueDecreases_When_NegativeArgumentPassed()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                float beforeDecrease = garrison.Count;
                garrison.PopFromGarrison(-5);
                float afterDecrease = garrison.Count;

                Assert.Greater(beforeDecrease, afterDecrease);
            }

            [Test]
            public void CountValueDoesntChange_When_ZeroPassed()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                float beforeDecrease = garrison.Count;
                garrison.PopFromGarrison(0);
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
                TowerGarrison garrison = new TowerGarrison(tower);

                //garrison.Count = -5;

                Assert.AreEqual(0, garrison.Count);
            }

            [Test]
            public void CountValueSets_WithoutModifications()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                //garrison.Count = 123456;

                Assert.AreEqual(123456, garrison.Count);
            }
        }

        public class OnAllyCame
        {
            [Test]
            public void IncreasesCountValueByOne_When_Called()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                float firstValue = garrison.Count;
                //garrison.OnAllyCame();
                float secondValue = garrison.Count;

                Assert.AreEqual(1, secondValue - firstValue);
            }
        }

        public class OnTowerAttacked
        {
            [Test]
            public void GarrisonCountLowers_When_Called()
            {
                ITower tower = Substitute.For<ITower>();
                tower.AttackInTower.Returns(5f);
                TowerGarrison garrison = new TowerGarrison(tower);
                IModel model = Substitute.For<IModel>();
                model.Attack.Returns(5f);
                model.HP.Returns(5f);

                float firstValue = garrison.Count;
                garrison.OnTowerAttacked(model);
                float secondValue = garrison.Count;

                Assert.Greater(firstValue, secondValue);
            }

            [Test]
            public void GarrisonCountIsTwo_When_ModelHPIsTwoModelAttackIsFourAndTowerAttackIsOneTowerHPIsOne()
            {
                ITower tower = Substitute.For<ITower>();
                tower.AttackInTower.Returns(1f);
                tower.HP.Returns(1f);
                TowerGarrison garrison = new TowerGarrison(tower);
                IModel model = Substitute.For<IModel>();
                model.Attack.Returns(4f);
                model.HP.Returns(2f);

                float firstValue = garrison.Count;
                garrison.OnTowerAttacked(model);
                float secondValue = garrison.Count;

                Assert.AreEqual(2, garrison.Count);
            }
        }

        public class CalculateNumberOfAttacks
        {
            [Test]
            public void ThreeAttacks_When_ModelHPIsFiveAndTowerAttackIsTwo()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                int numberOfAttacks = garrison.CalculateNumberOfAttacks(5, 2);

                Assert.AreEqual(3, numberOfAttacks);
            }

            [Test]
            public void TwoAttacks_When_HPIsFourAndAttacksIsTwo()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                int numberOfAttacks = garrison.CalculateNumberOfAttacks(4, 2);

                Assert.AreEqual(2, numberOfAttacks);
            }

            [Test]
            public void OneAttack_When_HPIsLessThanAttack()
            {
                ITower tower = Substitute.For<ITower>();
                TowerGarrison garrison = new TowerGarrison(tower);

                int numberOfAttacks = garrison.CalculateNumberOfAttacks(1, 4);

                Assert.AreEqual(1, numberOfAttacks);
            }
        }
    }
}
