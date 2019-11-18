using System;
using BatailleNavale.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static BatailleNavale.Controller.MainMenuController;

namespace BatailleNavaleTest.Controller
{
    [TestClass]
    public class MainMenuControllerTest
    {
        MainMenuController controller = new MainMenuController();

        [TestMethod]
        public void NewGameTest()
        {
            GameSettings settings = new GameSettings
            {
                GameMode = GameMode.Singleplayer,
                BoatCount = 5,
                Difficulty = BatailleNavale.Model.IAModel.Difficulty.None
            };

            controller.NewGame(settings);
        }

        [TestMethod]
        public void ShowSettingsTest()
        {
            controller.ShowSettings();
        }

        [TestMethod]
        public void SaveSettingsTest()
        {
            Assert.IsTrue(controller.SaveSettings(out _));
        }
    }
}
