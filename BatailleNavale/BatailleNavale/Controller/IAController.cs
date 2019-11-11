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
            this.GameController = controller;
            this.IAModel = new IAModel(difficulty);
        }

        /// <summary>
        /// Get the next IA target.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetNextTarget()
        {
            Vector2 target = Vector2.Zero;

            Vector2 latestHit = GameController.PlayerGrid.Hits.Last();
            Random rng = new Random();

            //IA Logic...
            switch (IAModel.Difficulty_) {
                case IAModel.Difficulty.None:
                    do
                    {
                        target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                    } while (GameController.PlayerGrid.HitExists(target));
                    break;
                case IAModel.Difficulty.Easy:
                    if (GameController.PlayerGrid.BoatExists(latesthit))
                    {
                        target = new Vector2(latestHit.X + 1, latestHit.Y);
                    }
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
