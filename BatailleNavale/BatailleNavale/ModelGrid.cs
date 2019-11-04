using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    class ModelGrid
    {
        private int size { get; set; }
        private List<ModelBateau> bateaux { get; set; }
        private List<int> coups { get; set; }

        public ModelGrid(int size, List<ModelBateau> bateaux, List<int> coups)
        {
            this.size = size;
            this.bateaux = bateaux;
            this.coups = coups;
        }
    }
}
