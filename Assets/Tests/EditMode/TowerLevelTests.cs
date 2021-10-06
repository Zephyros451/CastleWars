using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TowerLevelTests
    {
        TowerLevel towerLevel;

        public TowerLevelTests()
        {
            GameObject gameObject = new GameObject();
            Tower tower = gameObject.AddComponent<Tower>();
            towerLevel = new TowerLevel(tower, 1);
        }

        [Test]
        public void ResetSetsLevelValueToZero()
        {
            towerLevel.Reset();
            Assert.AreEqual(0, towerLevel.Value);
        }
    }
}
