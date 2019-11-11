using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class ModelUser
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int NbrVictories { get; set; }
        public int NbrLoses { get; set; }

        public ModelUser(string name, int score, int nbrVictories, int nbrLoses)
        {
            this.Name = name;
            this.Score = score;
            this.NbrVictories = nbrVictories;
            this.NbrLoses = nbrLoses;
        }
    }
}
