﻿using NSubstitute;
using NUnit.Framework;

namespace Tests
{
    public class TowerLevelTests
    {
        public class Reset
        {
            [Test]
            public void LevelValueSetsToZero_When_Called()
            {
                ITower tower = Substitute.For<ITower>();
                TowerLevel towerLevel = new TowerLevel(tower, 0);

                towerLevel.Reset();

                Assert.AreEqual(0, towerLevel.Value);
            }
        }

        public class LevelUp
        {
            [Test]
            public void LevelValueIncreasesByOne_When_Called()
            {
                ITower tower = Substitute.For<ITower>();
                TowerLevel towerLevel = new TowerLevel(tower, 0);
                tower.GarrisonCount.Returns(10f);
                tower.LvlUpQuantity.Returns(5);

                int levelValue = towerLevel.Value;
                towerLevel.LevelUp();
                int newLevelValue = towerLevel.Value;

                Assert.AreEqual(1, newLevelValue - levelValue);

                towerLevel.LevelUp();
                newLevelValue = towerLevel.Value;

                Assert.AreEqual(2, newLevelValue - levelValue);
            }
        }        
    }
}
