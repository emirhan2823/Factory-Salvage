using NUnit.Framework;
using FactorySalvage.Gameplay;

namespace FactorySalvage.Tests
{
    public class GridTests
    {
        #region CellData Tests

        [Test]
        public void CellData_Ground_Is_Buildable()
        {
            var cell = CellData.CreateGround();

            Assert.IsTrue(cell.IsBuildable);
            Assert.IsTrue(cell.IsWalkable);
        }

        [Test]
        public void CellData_Wall_Is_Not_Buildable()
        {
            var cell = CellData.CreateWall();

            Assert.IsFalse(cell.IsBuildable);
            Assert.IsFalse(cell.IsWalkable);
        }

        [Test]
        public void CellData_Occupied_Is_Not_Buildable_And_Not_Walkable()
        {
            var cell = new CellData { Type = CellType.Occupied };

            Assert.IsFalse(cell.IsBuildable);
            Assert.IsFalse(cell.IsWalkable);
        }

        [Test]
        public void CellData_Empty_Is_Buildable()
        {
            var cell = new CellData { Type = CellType.Empty };

            Assert.IsTrue(cell.IsBuildable);
        }

        #endregion

        #region CellType Tests

        [Test]
        public void CellType_Has_All_Expected_Values()
        {
            Assert.AreEqual(0, (int)CellType.Empty);
            Assert.AreEqual(1, (int)CellType.Ground);
            Assert.AreEqual(2, (int)CellType.Wall);
            Assert.AreEqual(3, (int)CellType.Occupied);
            Assert.AreEqual(4, (int)CellType.SpawnPoint);
            Assert.AreEqual(5, (int)CellType.Base);
        }

        #endregion
    }
}
