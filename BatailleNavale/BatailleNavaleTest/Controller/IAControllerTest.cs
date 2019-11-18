using System;
using BatailleNavale.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BatailleNavaleTest.Controllers
{
    [TestClass]
    public class IAControllerTest
    {
        static SingleplayerGameController singleplayerGameController = new SingleplayerGameController(BatailleNavale.Model.IAModel.Difficulty.None);
        IAController controller = new IAController(singleplayerGameController, BatailleNavale.Model.IAModel.Difficulty.None);

        [TestMethod]
        public void GetNextTargetTest()
        {
            controller.GetNextTarget();
        }
    }
}
