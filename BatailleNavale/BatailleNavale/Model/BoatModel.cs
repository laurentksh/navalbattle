using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class BoatModel
    {
        public const int MaxSize = 4;

        public Vector2 Position { get; set; }
        public int Size { get; set; }
        public Orientation Orientation_ { get; set; }
        public string Name { get; set; }

        public BoatModel(Vector2 pos, int size, Orientation orientation, string name)
        {
            Position = pos;
            Size = size;
            Orientation_ = orientation;
            Name = name;
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }
}
