using System;
using BatailleNavale.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BatailleNavaleTest.Controllers
{
    [TestClass]
    public class IAControllerTest
    {
        static GameController GameController;
        IAController controller = new IAController(GameController, BatailleNavale.Model.IAModel.Difficulty.None);

        [TestMethod]
        public void GetNextTargetTest()
        {
            controller.GetNextTarget();
        }
    }
}
