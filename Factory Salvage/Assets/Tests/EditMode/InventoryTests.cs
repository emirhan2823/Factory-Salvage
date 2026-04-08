using NUnit.Framework;
using UnityEngine;
using FactorySalvage.Data;
using FactorySalvage.Gameplay;

namespace FactorySalvage.Tests
{
    public class InventoryTests
    {
        #region Fields

        private GameObject _inventoryGo;
        private Inventory _inventory;
        private ResourceDefinition _scrapMetal;
        private ResourceDefinition _wire;

        #endregion

        #region Setup

        [SetUp]
        public void SetUp()
        {
            _inventoryGo = new GameObject("Inventory");
            _inventory = _inventoryGo.AddComponent<Inventory>();

            _scrapMetal = ScriptableObject.CreateInstance<ResourceDefinition>();
            _scrapMetal.name = "ScrapMetal";

            _wire = ScriptableObject.CreateInstance<ResourceDefinition>();
            _wire.name = "Wire";
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_inventoryGo);
            Object.DestroyImmediate(_scrapMetal);
            Object.DestroyImmediate(_wire);
        }

        #endregion

        #region Tests

        [Test]
        public void AddResource_Increases_Amount()
        {
            _inventory.AddResource(_scrapMetal, 10);

            Assert.AreEqual(10, _inventory.GetAmount(_scrapMetal));
        }

        [Test]
        public void AddResource_Stacks_Multiple_Adds()
        {
            _inventory.AddResource(_scrapMetal, 5);
            _inventory.AddResource(_scrapMetal, 3);

            Assert.AreEqual(8, _inventory.GetAmount(_scrapMetal));
        }

        [Test]
        public void RemoveResource_Decreases_Amount()
        {
            _inventory.AddResource(_scrapMetal, 10);
            bool result = _inventory.RemoveResource(_scrapMetal, 4);

            Assert.IsTrue(result);
            Assert.AreEqual(6, _inventory.GetAmount(_scrapMetal));
        }

        [Test]
        public void RemoveResource_Fails_When_Insufficient()
        {
            _inventory.AddResource(_scrapMetal, 5);
            bool result = _inventory.RemoveResource(_scrapMetal, 10);

            Assert.IsFalse(result);
            Assert.AreEqual(5, _inventory.GetAmount(_scrapMetal));
        }

        [Test]
        public void HasEnoughResource_Returns_Correct()
        {
            _inventory.AddResource(_scrapMetal, 10);

            Assert.IsTrue(_inventory.HasEnoughResource(_scrapMetal, 10));
            Assert.IsTrue(_inventory.HasEnoughResource(_scrapMetal, 5));
            Assert.IsFalse(_inventory.HasEnoughResource(_scrapMetal, 11));
        }

        [Test]
        public void GetAmount_Returns_Zero_For_Empty()
        {
            Assert.AreEqual(0, _inventory.GetAmount(_scrapMetal));
        }

        [Test]
        public void Multiple_Resources_Tracked_Independently()
        {
            _inventory.AddResource(_scrapMetal, 10);
            _inventory.AddResource(_wire, 5);

            Assert.AreEqual(10, _inventory.GetAmount(_scrapMetal));
            Assert.AreEqual(5, _inventory.GetAmount(_wire));
        }

        [Test]
        public void HasEnoughResources_Checks_Multiple_Costs()
        {
            _inventory.AddResource(_scrapMetal, 10);
            _inventory.AddResource(_wire, 5);

            var costs = new ResourceCost[]
            {
                new() { Resource = _scrapMetal, Amount = 5 },
                new() { Resource = _wire, Amount = 3 }
            };

            Assert.IsTrue(_inventory.HasEnoughResources(costs));

            var expensiveCosts = new ResourceCost[]
            {
                new() { Resource = _scrapMetal, Amount = 5 },
                new() { Resource = _wire, Amount = 10 }
            };

            Assert.IsFalse(_inventory.HasEnoughResources(expensiveCosts));
        }

        [Test]
        public void SpendResources_Deducts_All()
        {
            _inventory.AddResource(_scrapMetal, 10);
            _inventory.AddResource(_wire, 5);

            var costs = new ResourceCost[]
            {
                new() { Resource = _scrapMetal, Amount = 3 },
                new() { Resource = _wire, Amount = 2 }
            };

            bool result = _inventory.SpendResources(costs);

            Assert.IsTrue(result);
            Assert.AreEqual(7, _inventory.GetAmount(_scrapMetal));
            Assert.AreEqual(3, _inventory.GetAmount(_wire));
        }

        [Test]
        public void Clear_Removes_Everything()
        {
            _inventory.AddResource(_scrapMetal, 10);
            _inventory.AddResource(_wire, 5);
            _inventory.Clear();

            Assert.AreEqual(0, _inventory.GetAmount(_scrapMetal));
            Assert.AreEqual(0, _inventory.GetAmount(_wire));
        }

        #endregion
    }
}
