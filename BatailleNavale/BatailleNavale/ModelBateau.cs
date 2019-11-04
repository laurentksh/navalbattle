﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    class ModelBateau
    {
        private int x { get; set; }
        private int y { get; set; }
        private int size { get; set; }
        private Orientation orientation { get; set; }

        public ModelBateau(int x, int y, int size, Orientation orientation)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.orientation = orientation;
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }
}