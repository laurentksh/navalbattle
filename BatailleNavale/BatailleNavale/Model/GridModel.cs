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
        public List<Hit> Hits { get; set; }

        public GridModel()
        {
            Boats = new List<BoatModel>();
            Hits = new List<Hit>();
        }

        public bool BoatExists(Vector2 pos)
        {
            foreach (BoatModel boat in Boats) {
                for (int i = 0; i < boat.Size; i++) { 
                    if (boat.Orientation_ == BoatModel.Orientation.Horizontal) {
                        if ((boat.Position.X + i) == pos.X && boat.Position.Y == pos.Y)
                            return true;
                    } else {
                        if ((boat.Position.Y + i) == pos.Y && boat.Position.X == pos.X)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool HitExists(Vector2 pos)
        {
            foreach (Hit hit in Hits) {
                if (hit.Position.X == pos.X && hit.Position.Y == pos.Y)
                    return true;
            }

            return false;
        }

        public bool HitExists(Hit hit) => HitExists(hit.Position);

        public List<BoatModel> GetDestroyedBoats()
        {
            List<BoatModel> destroyedBoats = new List<BoatModel>();

            foreach (BoatModel boat in Boats) {
                int hits = boat.GetDestroyedCases(Hits);

                if (hits == boat.Size)
                    destroyedBoats.Add(boat);
            }

            return destroyedBoats;
        }
    }

    public class Hit
    {
        public DateTime CurrentDateTime;
        public Vector2 Position;

        public Hit()
        {
            CurrentDateTime = DateTime.Now;
        }
    }
}
