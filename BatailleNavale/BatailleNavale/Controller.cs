using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    class Controller
    {
        public List<ModelBoat> boats = new List<ModelBoat>();

        public void CreateBoat (int x, int y, int size, ModelBoat.Orientation orientation, string name)
        {
            ModelBoat boat = new ModelBoat(x, y, size, orientation, name);
            boats.Add(boat);
        }

        public void CreateGrid((int, int) grid)
        {

        }
    }
}
