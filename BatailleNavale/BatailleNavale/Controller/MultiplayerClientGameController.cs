using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using BatailleNavale.Model;
using BatailleNavale.View;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using BatailleNavale.Net;

namespace BatailleNavale.Controller
{
    public class MultiplayerClientGameController : IGameController
    {
        public GameMode GameMode { get; set; }
        public bool? Host { get; set; }
        public GameState GameState { get; set; }
        public GameResult Result { get; set; }

        public MainMenuController MainMenuController { get; set; }

        public GameWindow GameView { get; set; }

        public GridModel PlayerGrid { get; set; }
        public GridModel EnemyGrid { get; set; }

        public NetworkCommunicator NetCom;

        private Stopwatch DurationSW;

        public MultiplayerClientGameController(MainMenuController mainMenuController)
        {
            GameMode = GameMode.Multiplayer;
            Host = false;
            GameState = GameState.PlayersChooseBoatsLayout;
            Result = GameResult.Interupted;

            NetCom = new NetworkCommunicator();

            MainMenuController = mainMenuController;
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();

            DurationSW = new Stopwatch();

            GameView = new GameWindow(this);
            GameView.Show();
        }

        public void SetReady()
        {
            DurationSW.Start();
            ChangeGameState(GameState.Player1Turn);

            NetCom.SetReady();
        }

        public void ChangeGameState(GameState state)
        {
            throw new NotImplementedException();
        }

        public void ProcessPlayerHit(Vector2 pos)
        {
            if (GameState != GameState.GameEnded)
                return;

            NetCom.Hit(new Hit(pos));

            GameView.DisplayHit(pos, Player.Player2); //Display a hitmarker where the player clicked.

            GameView.SetGridIsEnabled(Player.Player1, true);
            GameView.SetGridIsEnabled(Player.Player2, false);

            ChangeGameState(GameState.Player2Turn);
        }

        public void PlayerHit(Vector2 pos, out Player winner)
        {
            throw new NotImplementedException();
        }

        public void EnemyHit(Vector2 pos, out Player winner)
        {
            throw new NotImplementedException();
        }

        public void GameWon(Player player)
        {
            throw new NotImplementedException();
        }

        public void QuitGame(GameResult result)
        {
            DurationSW.Stop();

            if (result != GameResult.Interupted) {

                GameData game = new GameData
                {
                    GameMode = GameMode,
                    Result = result,

                    LocalPlayerBoats = PlayerGrid.Boats,
                    EnemyBoats = EnemyGrid.Boats,
                    LocalPlayerHits = PlayerGrid.Hits,
                    EnemyHits = EnemyGrid.Hits,

                    Chat = GameView.ChatContentTB.Text,
                    Duration = DurationSW.Elapsed
                };

                MainMenuController.RegisterGame(game);
            }

            MainMenuController.SetInGame(false);
            MainMenuController.MainMenuView.Activate();
            GameView.Close();
        }
    }
}
