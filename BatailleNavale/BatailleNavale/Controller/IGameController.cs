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
        GameMode GameMode { get; set; }

        bool? Host { get; set; }

        /// <summary>The current state of  the game.</summary>
        GameState GameState { get; set; }

        /// <summary>Game result.</summary>
        GameResult Result { get; set; }


        /// <summary>MainMenuController reference (mostly to save the game at the end).</summary>
        MainMenuController MainMenuController { get; set; }

        /// <summary>Where the game will be displayed.</summary>
        GameWindow GameView { get; set; }

        /// <summary>Grid of the player 1 in memory.</summary>
        GridModel PlayerGrid { get; set; }
        /// <summary>Grid of the player 2 in memory.</summary>
        GridModel EnemyGrid { get; set; }
        

        /// <summary>Warn the host controller that we are ready. (Client and Host)</summary>
        void SetReady();

        /// <summary></summary>
        /// <param name="state"></param>
        void ChangeGameState(GameState state);

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

        /// <summary>Quit game and signal the main menu controller that the game ended. (Client and Host)</summary>
        /// <param name="result"></param>
        void QuitGame(GameResult result);
    }

    public enum GameState
    {
        /// <summary>Players choose their grid layout.</summary>
        PlayersChooseBoatsLayout,
        /// <summary>Player 1 turn.</summary>
        Player1Turn,
        /// <summary>Player 2 turn.</summary>
        Player2Turn,
        /// <summary>The game has ended. Players can now safely quit the window. The game chat may stlil be enabled.</summary>
        GameEnded
    }

    public enum GameResult
    {
        LocalPlayerWon,
        EnemyWon,
        Draw,
        Interupted,
    }

    public enum Player
    {
        None = 0,
        Player1 = 1,
        Player2 = 2
    }
}