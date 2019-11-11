using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    public class ModelBoat
    {
        private int X { get; set; }
        private int Y { get; set; }
        private int Size { get; set; }
        private Orientation Orientation_ { get; set; }
        private string Name { get; set; }

        public ModelBoat(int X, int y, int size, Orientation orientation, string name)
        {
            this.X = X;
            this.Y = Y;
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
