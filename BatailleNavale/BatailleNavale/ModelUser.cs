using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale
{
    class ModelUser
    {
        private string name { get; set; }
        private int score { get; set; }
        private int nbrVictories { get; set; }
        private int nbrLoses { get; set; }

        public ModelUser(string name, int score, int nbrVictories, int nbrLoses)
        {
            this.name = name;
            this.score = score;
            this.nbrVictories = nbrVictories;
            this.nbrLoses = nbrLoses;
        }
    }
}
