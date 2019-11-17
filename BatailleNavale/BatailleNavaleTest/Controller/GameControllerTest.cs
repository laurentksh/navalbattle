using System;
using System.Numerics;
using BatailleNavale.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BatailleNavaleTest.Controller
{
    [TestClass]
    public class GameControllerTest
    {
        GameController controller = new GameController(BatailleNavale.Model.IAModel.Difficulty.None);

        [TestMethod]
        public void CreateBoatTest()
        {
            controller.CreateBoat(true, Vector2.Zero, 2, BatailleNavale.Model.BoatModel.Orientation.Horizontal);
        }

        [TestMethod]
        public void GenerateBoatsTest()
        {
            controller.GenerateBoats(5, true);
        }

        [TestMethod]
        public void PlayerHitTest()
        {
            controller.PlayerHit(Vector2.Zero);
        }

        [TestMethod]
        public void EnemyHit()
        {
            controller.EnemyHit(Vector2.Zero);
        }
    }
}
