using BatailleNavale.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Controller
{
    public class IAController
    {
        private GameController GameController;
        private IAModel IAModel;

        public IAController(GameController controller, IAModel.Difficulty difficulty)
        {
            GameController = controller;
            IAModel = new IAModel(difficulty);
        }

        /// <summary>
        /// Get the next IA target.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNextTarget()
        {
            Vector2 target = Vector2.Zero;

            Vector2 latestHit = GameController.EnemyGrid.Hits.Last();
            Random rng = new Random();

            //IA Logic...
            switch (IAModel.Difficulty_) {
                case IAModel.Difficulty.None:
                    target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                    break;
                case IAModel.Difficulty.Easy:
                    break;
                case IAModel.Difficulty.Normal:
                    break;
                case IAModel.Difficulty.Hard:
                    break;
                default:
                    break;
            }

            return target;
        }
    }
}
