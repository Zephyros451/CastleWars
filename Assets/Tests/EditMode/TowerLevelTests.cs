using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using NSubstitute;

namespace Tests
{
    public class TowerLevelTests
    {
        TowerLevel towerLevel;
        ITower tower;

        public TowerLevelTests()
        {
            tower = Substitute.For<ITower>();
            towerLevel = new TowerLevel(tower, 0);
        }

        [Test]
        public void SettingStartingLevelThroughConstructor()
        {
            Assert.AreEqual(5, new TowerLevel(null, 5).Value);
            Assert.AreEqual(0, new TowerLevel(null, 0).Value);
            Assert.AreEqual(0, new TowerLevel(null, -5).Value);
        }

        [Test]
        public void ResetSetsLevelValueToZero()
        {
            towerLevel.Reset();
            Assert.AreEqual(0, towerLevel.Value);
        }

        [Test]
        public void LevelUpIncreasesLevelByOne()
        {
            var garrison = new TowerGarrison();
            tower.Garrison.Returns(garrison);
            tower.Garrison.Count = 10f;
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
