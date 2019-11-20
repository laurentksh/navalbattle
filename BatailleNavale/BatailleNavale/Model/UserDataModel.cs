using BatailleNavale.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static BatailleNavale.Controller.MainMenuController;

namespace BatailleNavale.Model
{
    public class UserDataModel //TODO: Fix bitwise AND operations.
    {
        /// <summary>
        /// Default UserDataModel
        /// </summary>
        public UserDataModel()
        {
            Random rnd = new Random();

            Username = "Player";
            Port = 9451;
            Games = new List<GameData>();
            Protocol = ProtocolType.Tcp;
        }

        //Player infos
        public string Username;

        //Server settings
        public int Port;
        public ProtocolType Protocol;

        //Player stats

        public List<GameData> Games;

        public void ResetStats()
        {
            Games = new List<GameData>();
        }

        public int GetBestScore(GameMode gameMode = GameMode.Both)
        {
            var wins = from game in Games
                       where game.Result == GameResult.LocalPlayerWon && (game.GameMode & gameMode) == gameMode
                       orderby game.Score ascending
                       select game;

            return wins.First().Score;
        }

        public int GetWins(GameMode gameMode = GameMode.Both)
        {
            var wins = from game in Games
                       where game.Result == GameResult.LocalPlayerWon && (game.GameMode & gameMode) == gameMode
                       select game;

            return wins.Count();
        }

        public int GetLoses(GameMode gameMode = GameMode.Both)
        {
            var looses = from game in Games
                         where game.Result == GameResult.LocalPlayerWon && (game.GameMode & gameMode) == gameMode
                         select game;

            return looses.Count();
        }

        public TimeSpan GetTotalPlayTime(GameMode gameMode = GameMode.Both)
        {
            TimeSpan result = new TimeSpan();

            foreach (GameData game in Games) {
                if ((game.GameMode & gameMode) == gameMode)
                    continue;

                result = result.Add(game.Duration);
            }

            return result;
        }
    }

    public class GameData
    {
        public GameResult Result;
        public int Score;
        public GameMode GameMode;
        public TimeSpan Duration;
    }

    [Flags]
    public enum GameMode
    {
        Singleplayer = 1,
        Multiplayer = 2,
        Both = Singleplayer | Multiplayer
    }
}
