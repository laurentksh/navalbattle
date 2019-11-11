using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class UserDataModel
    {
        public UserDataModel()
        {
            Username = "Player";
            Port = 9451;
            Score = 0;
            Wins = 0;
            Looses = 0;
        }

        //Player infos
        public string Username;

        //Server settings
        public int Port;
        

        //Player stats

        /// <summary>Amount of hits in the player's best game.</summary>
        public int Score;
        public int Wins;
        public int Looses;
    }
}