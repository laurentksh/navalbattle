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
        private SingleplayerGameController GameController;
        private IAModel IAModel;

        public IAController(SingleplayerGameController controller, IAModel.Difficulty difficulty)
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
            Random rng = new Random();

            if (GameController.PlayerGrid.Hits.Count == 0) {
                target = new Vector2(rng.Next(0, GridModel.SizeX -1), rng.Next(0, GridModel.SizeY - 1));
                return target;
            }

            Vector2 latestHit = GameController.PlayerGrid.Hits.Last();

            //IA Logic...
            switch (IAModel.Difficulty_) {
                case IAModel.Difficulty.None:
                    do {
                        target = new Vector2(rng.Next(0, GridModel.SizeX - 1), rng.Next(0, GridModel.SizeY - 1));
                    } while (GameController.PlayerGrid.HitExists(target));
                    break;
                case IAModel.Difficulty.Easy:
                    if (GameController.PlayerGrid.BoatExists(latestHit)) {
                        target = new Vector2(latestHit.X + 1, latestHit.Y); //TODO: Needs improvement
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

        /// <summary>
        /// Generate boats for the local player.
        /// </summary>
        /// <param name="boatCount"></param>
        public void GenerateBoats(int boatCount)
        {
            List<Vector2> usedPositions = new List<Vector2>();
            Random rnd = new Random();
            Vector2 pos;

            for (int i = 0; i < boatCount; i++) { //Random boat generation for now, will change later.
                do {
                    pos = new Vector2(rnd.Next(GridModel.SizeX), rnd.Next(GridModel.SizeY));
                } while (usedPositions.Contains(pos));

                usedPositions.Add(pos);

                GameController.CreateBoat(false, pos, rnd.Next(BoatModel.MaxSize), (BoatModel.Orientation)rnd.Next(1));
            }
        }
    }
}
