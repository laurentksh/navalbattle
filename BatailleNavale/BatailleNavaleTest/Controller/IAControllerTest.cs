﻿using System;
using BatailleNavale.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BatailleNavaleTest.Controller
{
    [TestClass]
    public class IAControllerTest
    {
        SingleplayerGameController singleplayerGameController;
        IAController controller;

        public IAControllerTest()
        {

            singleplayerGameController = new SingleplayerGameController(new MainMenuController(), BatailleNavale.Model.IAModel.Difficulty.None);
            controller = new IAController(singleplayerGameController, BatailleNavale.Model.IAModel.Difficulty.None);
        }

        [TestMethod]
        public void GetNextTargetTest()
        {
            Assert.IsInstanceOfType(controller.GetNextTarget(), typeof(System.Numerics.Vector2));
        }
    }
}
