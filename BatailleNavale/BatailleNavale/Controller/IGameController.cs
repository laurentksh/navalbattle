using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.Model;
using BatailleNavale.View;

namespace BatailleNavale.Controller
{
    public interface IGameController
    {
        GameState GameState { get; set; }

        GameWindow GameView { get; set; }
        GridModel PlayerGrid { get; set; }
        GridModel EnemyGrid { get; set; }


        /// <summary>Warn the host controller that we are ready. (Client and Host)</summary>
        void SetReady();

        /// <summary>Process the player hit pre-actions. (Client and Host)</summary>
        /// <param name="pos"></param>
        void ProcessPlayerHit(Vector2 pos);

        /// <summary>Register a player hit and check if the game is won. (Host only)</summary>
        /// <param name="pos"></param>
        void PlayerHit(Vector2 pos, out Player winner);

        /// <summary>Register an enemy hit and check if the game is won. (Host only)</summary>
        /// <param name="pos"></param>
        void EnemyHit(Vector2 pos, out Player winner);

        /// <summary>Signal the other player that the game is won and display a UI message. (Host only)</summary>
        /// <param name="player"></param>
        void GameWon(Player player);
    }

    public enum GameState
    {
        PlayersChooseBoatsLayout,
        Player1Turn,
        Player2Turn,
        GameEnded
    }

    public enum Player
    {
        None = 0,
        Player1 = 1,
        Player2 = 2
    }
}