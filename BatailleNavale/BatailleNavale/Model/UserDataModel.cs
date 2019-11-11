using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class UserDataModel
    {
        public string Username;

        /// <summary>Amount of hits in the player's best game.</summary>
        public int Score;
        public int Wins;
        public int Looses;

        public int Port;
    }
}
