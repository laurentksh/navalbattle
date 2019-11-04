using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    public class ModelGrid
    {
        public const int SizeX = 10;
        public const int SizeY = 10;

        public List<ModelBoat> bateaux { get; set; }
        public List<Vector2> coups { get; set; }

        public ModelGrid(List<ModelBoat> bateaux, List<Vector2> coups)
        {
            this.bateaux = bateaux;
            this.coups = coups;
        }
    }
}
