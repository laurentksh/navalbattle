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
        public IAModel IAModel;

        private SingleplayerGameController GameController;

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
            //NOTE: maxValue is exclusive !!

            Vector2 target = Vector2.Zero;
            Random rng = new Random();
            int offsetX;
            int offsetY;
#pragma warning disable
            int direction = 0;
#pragma warning restore

            if (GameController.PlayerGrid.Hits.Count == 0) {
                switch (IAModel.Difficulty_) {
                    default:
                    case IAModel.Difficulty.None:
                        target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                        break;
                    case IAModel.Difficulty.Easy:
                        target = new Vector2(rng.Next(0, GridModel.SizeX - 5), rng.Next(0, GridModel.SizeY - 5));
                        break;
                    case IAModel.Difficulty.Normal:
                        target = new Vector2(rng.Next(0, GridModel.SizeX - 4), rng.Next(0, GridModel.SizeY - 4));
                        break;
                    case IAModel.Difficulty.Hard:
                        target = new Vector2(rng.Next(3, GridModel.SizeX - 3), rng.Next(3, GridModel.SizeY - 3));
                        break;
                }

                return target;
            }

            List<Hit> latestHits = GameController.PlayerGrid.Hits;
            latestHits.Reverse();

            //IA Logic...
            switch (IAModel.Difficulty_) {
                default:
                case IAModel.Difficulty.None:
                    do {
                        target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                    } while (GameController.PlayerGrid.HitExists(target));
                    break;
                case IAModel.Difficulty.Easy:
                    if (GameController.PlayerGrid.BoatExists(latestHits[1].Position) && GameController.PlayerGrid.BoatExists(latestHits[0].Position)) {
                        offsetX = (int)latestHits[0].Position.X - (int)latestHits[1].Position.X;
                        offsetY = (int)latestHits[0].Position.Y - (int)latestHits[1].Position.Y;
                        target = new Vector2(latestHits[0].Position.X + offsetX, latestHits[0].Position.Y + offsetY);

                    } else if (GameController.PlayerGrid.BoatExists(latestHits[1].Position) && !GameController.PlayerGrid.BoatExists(latestHits[0].Position)) {//TODO : Change offsets via direction
                        offsetX = 0;
                        offsetY = 0;
                        target = new Vector2(latestHits[1].Position.X + offsetX, latestHits[1].Position.Y + offsetY);
                    }
                    if (GameController.PlayerGrid.BoatExists(latestHits[0].Position)) {
                        target = new Vector2(latestHits[0].Position.X + 1, latestHits[0].Position.Y); //TODO: Needs improvement
                    } else {
                        do {
                            target = new Vector2(rng.Next(0, GridModel.SizeX - 1), rng.Next(0, GridModel.SizeY - 1));
                        } while (GameController.PlayerGrid.HitExists(target));
                    }
                    break;
                case IAModel.Difficulty.Normal:
                    do {
                        target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                    } while (GameController.PlayerGrid.HitExists(target));
                    break;
                case IAModel.Difficulty.Hard:
                    do {
                        target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                    } while (GameController.PlayerGrid.HitExists(target));
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
            List<BoatPreset> boatPresets = BoatModel.GetBoatPresets();

            for (int i = 0; i < boatPresets.Count; i++) {
                BoatPreset boatPreset = boatPresets[i];
                Vector2 pos = Vector2.Zero;
                BoatModel.Orientation orientation = (BoatModel.Orientation)rnd.Next(2);
                int X = rnd.Next(GridModel.SizeX);
                int Y = rnd.Next(GridModel.SizeY);

                do {
                    if (orientation == BoatModel.Orientation.Horizontal)
                        X = rnd.Next(GridModel.SizeX - boatPreset.boatSize);
                    else
                        Y = rnd.Next(GridModel.SizeY - boatPreset.boatSize);

                    pos = new Vector2(X, Y);
                } while (usedPositions.Contains(pos));

                for (int i2 = 0; i2 < boatPreset.boatSize; i2++) {
                    if (orientation == BoatModel.Orientation.Horizontal)
                        usedPositions.Add(new Vector2(pos.X + i2, pos.Y));
                    else
                        usedPositions.Add(new Vector2(pos.X, pos.Y + i2));
                }

                GameController.CreateBoat(Player.Player2, pos, boatPreset.boatSize, orientation, boatPreset.boatId);
            }
        }
    }
}
