﻿using BatailleNavale.Model;
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
                    case IAModel.Difficulty.Easy:
                        target = new Vector2(rng.Next(0, GridModel.SizeX), rng.Next(0, GridModel.SizeY));
                        break;
                    case IAModel.Difficulty.Normal:
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
                    break;
                case IAModel.Difficulty.Hard:
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

                GameController.CreateBoat(Player.Player2, pos, rnd.Next(BoatModel.MaxSize), (BoatModel.Orientation)rnd.Next(1));
            }
        }
    }
}
