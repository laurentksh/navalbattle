using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BatailleNavale.Model
{
    public class ModelGrid
    {
        public const int SizeX = 10;
        public const int SizeY = 10;

        public List<ModelBoat> Boats { get; set; }
        public List<Vector2> Hits { get; set; }
        public State State_ { get; set; }

        public ModelGrid(List<ModelBoat> bateaux, List<Vector2> coups)
        {
            this.Boats = bateaux;
            this.Hits = coups;

            this.State_ = State.WarmUp;
        }

        public bool HitExists(Vector2 pos)
        {
            foreach (Vector2 hit in Hits) {
                if (hit.X == pos.X && hit.Y == pos.Y)
                    return true;
            }

            return false;
        }

        public enum State
        {
            WarmUp,
            InGame,
            EndGame
        }
    }
}
