using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace BatailleNavale.Model
{
    public class GridModel
    {
        public const int SizeX = 10;
        public const int SizeY = 10;

        public List<BoatModel> Boats { get; set; }
        public List<Vector2> Hits { get; set; }

        public GridModel()
        {
            Boats = new List<BoatModel>();
            Hits = new List<Vector2>();
        }

        public bool BoatExists(Vector2 pos)
        {
            foreach (BoatModel boat in Boats) {
                for (int i = 0; i < boat.Size; i++) {
                    if (boat.Orientation_ == BoatModel.Orientation.Horizontal) {
                        if ((boat.Position.X + i) == pos.X)
                            return true;
                    } else {
                        if ((boat.Position.Y + i) == pos.Y)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool HitExists(Vector2 pos)
        {
            foreach (Vector2 hit in Hits) {
                if (hit.X == pos.X && hit.Y == pos.Y)
                    return true;
            }

            return false;
        }

        public List<BoatModel> GetDestroyedBoats()
        {
            List<BoatModel> destroyedBoats = new List<BoatModel>();

            foreach (BoatModel boat in this.Boats) {
                int hits = boat.GetDestroyedCases(this.Hits);

                if (hits == boat.Size)
                    destroyedBoats.Add(boat);
            }

            return destroyedBoats;
        }
    }
}
