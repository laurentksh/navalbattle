using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    class Controller
    {
        List<ModelBateau> bateaux = new List<ModelBateau>();
        public void createBateau (int x, int y, int size, ModelBateau.Orientation orientation)
        {
            ModelBateau bateau = new ModelBateau(x, y, size, orientation);
            bateaux.Add(bateau);
        }
    }
}
