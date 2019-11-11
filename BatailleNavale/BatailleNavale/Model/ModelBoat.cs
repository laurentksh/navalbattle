using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class ModelBoat
    {
        public Vector2 Position { get; set; }
        public int Size { get; set; }
        public Orientation Orientation_ { get; set; }
        public string Name { get; set; }

        public ModelBoat(Vector2 pos, int size, Orientation orientation, string name)
        {
            this.Position = pos;
            this.Size = size;
            this.Orientation_ = orientation;
            this.Name = name;
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }
}
