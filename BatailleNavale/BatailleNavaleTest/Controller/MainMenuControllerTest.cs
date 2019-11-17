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
            controller.NewGame(GameMode.Singleplayer);
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
