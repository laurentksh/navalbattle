using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class UserDataModel
    {
        /// <summary>
        /// Default UserDataModel
        /// </summary>
        public UserDataModel()
        {
            Random rnd = new Random();

            Username = "Player#" + rnd.Next(1000, 9999);
            Port = 9451;
            BestScore = 0;
            Wins = 0;
            Looses = 0;
            Protocol = ProtocolType.Tcp;
        }

        //Player infos
        public string Username;

        //Server settings
        public int Port;
        public ProtocolType Protocol;

        //Player stats

        /// <summary>Amount of hits in the player's best game.</summary>
        public int BestScore;
        public int Wins;
        public int Looses;
    }
}