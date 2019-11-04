﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    class ModelBoat
    {
        private int x { get; set; }
        private int y { get; set; }
        private int size { get; set; }
        private Orientation orientation { get; set; }
        private string name { get; set; }

        public ModelBoat(int x, int y, int size, Orientation orientation, string name)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.orientation = orientation;
            this.name = name;
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }
}
