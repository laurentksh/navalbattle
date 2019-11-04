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
        private State etat { get; set; }
        private List<ModelBoat> bateaux { get; set; }
        private List<int> coups { get; set; }

        public ModelGrid(int size, State etat, List<ModelBoat> bateaux, List<int> coups)
        {
            this.size = size;
            this.bateaux = bateaux;
            this.coups = coups;
            this.etat = etat;
        }

        public enum State
        {
            WarmUp,
            InGame,
            EndGame
        }
    }
}
