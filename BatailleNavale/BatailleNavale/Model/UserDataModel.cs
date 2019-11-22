using BatailleNavale.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.IO;

namespace BatailleNavale.Model
{
    [Serializable]
    public class UserDataModel
    {
        /// <summary>
        /// Default UserDataModel
        /// </summary>
        public UserDataModel()
        {
            Username = "Player";
            ResetProfilePicture();
            Protocol = ProtocolType.Tcp;
            Port = 9451;

            Games = new List<GameData>();
        }

        /// <summary>Player's username</summary>
        public string Username;
        /// <summary>Player's profile picture (in Base64).</summary>
        public string ProfilePicture;

        //Server settings
        public int Port;
        public ProtocolType Protocol;

        public List<GameData> Games;

        public bool SetProfilePicture(string path)
        {
            return SetProfilePicture(System.Drawing.Image.FromFile(path));
        }

        public bool SetProfilePicture(System.Drawing.Image img)
        {
            try {
                using (MemoryStream ms = new MemoryStream()) {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    ProfilePicture = Convert.ToBase64String(ms.ToArray());
                }
            } catch (Exception) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reset the profile picture to the default one.
        /// </summary>
        public void ResetProfilePicture()
        {
            SetProfilePicture("./Resources/DefaultAvatar.png");
        }

        /// <summary>
        /// Deletes all saved games.
        /// </summary>
        public void ResetStats()
        {
            Games = new List<GameData>(); //Faster than .Clear() when you have a lot of items.
        }

        /// <summary>
        /// Enumarate across each saved games and return the win with the highest score.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public int GetBestScore(GameMode gameMode = GameMode.Both)
        {
            var wins = from game in Games
                       where game.Result == GameResult.LocalPlayerWon && game.GameMode.HasFlag(gameMode)
                       orderby game.LocalPlayerHits.Count descending
                       select game;

            if (wins.Count() == 0)
                return 0;

            return wins.First().LocalPlayerHits.Count;
        }

        /// <summary>
        /// Return the amount of wins in saved games.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public int GetWins(GameMode gameMode = GameMode.Both)
        {
            var wins = from game in Games
                       where game.Result == GameResult.LocalPlayerWon && game.GameMode.HasFlag(gameMode)
                       select game;

            return wins.Count();
        }

        /// <summary>
        /// Return the amount of losses in saved games.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public int GetLosses(GameMode gameMode = GameMode.Both)
        {
            var looses = from game in Games
                         where game.Result == GameResult.LocalPlayerWon && game.GameMode.HasFlag(gameMode)
                         select game;

            return looses.Count();
        }

        /// <summary>
        /// Return the total time played across all played games.
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
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

        public override string ToString()
        {
            return new SimpleUserDataModel(this).ToString();
        }
    }
    
    /// <summary>
    /// Object sent to other players.
    /// </summary>
    [Serializable]
    public class SimpleUserDataModel
    {
        public SimpleUserDataModel()
        {

        }

        public SimpleUserDataModel(UserDataModel udModel)
        {
            Username = udModel.Username;
            ProfilePicture = udModel.ProfilePicture;

            BestScore = udModel.GetBestScore();
            Wins = udModel.GetWins();
            Losses = udModel.GetLosses();
            TotalPlayTime = udModel.GetTotalPlayTime();
        }

        /// <summary>Player's username</summary>
        public string Username;
        /// <summary>Player's profile picture (in Base64).</summary>
        public string ProfilePicture;

        public int BestScore;
        public int Wins;
        public int Losses;
        public TimeSpan TotalPlayTime;

        public override string ToString()
        {
            string totalPlayTime;
            if (TotalPlayTime.TotalHours > 1d)
                totalPlayTime = $"Total play time: {TotalPlayTime.Hours}h";
            else if (TotalPlayTime.TotalMinutes > 1d)
                totalPlayTime = $"Total play time: {TotalPlayTime.Minutes}m";
            else
                totalPlayTime = $"Total play time: {TotalPlayTime.Seconds}s";

            float ratio = Wins;
            if (Losses != 0)
                ratio = Wins / Losses;

            return
                $"{Username}" + Environment.NewLine +
                $"W/L/Ratio: {Wins}/{Losses} - {ratio}" + Environment.NewLine +
                $"Best score: {BestScore}" + Environment.NewLine +
                totalPlayTime;
        }
    }

    public class GameData
    {
        public GameResult Result;
        public GameMode GameMode;
        public List<BoatModel> LocalPlayerBoats;
        public List<BoatModel> EnemyBoats;

        /// <summary>Hits on Player1's grid.</summary>
        public List<Hit> LocalPlayerHits;
        /// <summary>Hits on Player2's grid.</summary>
        public List<Hit> EnemyHits;

        public TimeSpan Duration;

        public override string ToString()
        {
            return $"{GameMode}.{Result}: P1: {LocalPlayerHits.Count} P2: {EnemyHits.Count} in {Duration.TotalSeconds} seconds.";
        }
    }

    [Flags]
    public enum GameMode
    {
        Singleplayer = 1,
        Multiplayer = 2,
        /// <summary>Used for HasFlag() operations. Please do not use this for anything else.</summary>
        Both = Singleplayer | Multiplayer
    }
}