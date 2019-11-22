using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using BatailleNavale.Model;
using BatailleNavale.View;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace BatailleNavale.Controller
{
    public class MultiplayerClientGameController : IGameController
    {
        public GameMode GameMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool? Host { get => false; set { } }
        public Player LocalPlayer { get => Player.Player2; set { } }
        public GameState GameState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GameResult Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MainMenuController MainMenuController { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public GameWindow GameView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public GridModel PlayerGrid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public GridModel EnemyGrid { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Socket HostConnection;

        private Stopwatch DurationSW;

        public MultiplayerClientGameController(MainMenuController mainMenuController)
        {
            MainMenuController = mainMenuController;
            GameState = GameState.PlayersChooseBoatsLayout;
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();

            DurationSW = new Stopwatch();

            Result = GameResult.Interupted;

            GameView = new GameWindow(this);
            GameView.Show();
        }

        public void SetReady()
        {
            //TODO: Tell the other player we're ready.

            DurationSW.Start();
            ChangeGameState(GameState.Player1Turn);
        }

        public void ChangeGameState(GameState state)
        {
            GameState = state;

            switch (GameState) {
                case GameState.PlayersChooseBoatsLayout:
                    GameView.SetGridIsEnabled(Player.Player1, true);
                    GameView.SetGridIsEnabled(Player.Player2, false);
                    break;
                case GameState.Player1Turn:
                    GameView.SetGridIsEnabled(Player.Player1, false);
                    GameView.SetGridIsEnabled(Player.Player2, true);
                    break;
                case GameState.Player2Turn:
                    GameView.SetGridIsEnabled(Player.Player1, true);
                    GameView.SetGridIsEnabled(Player.Player2, false);
                    break;
                case GameState.GameEnded:
                    GameView.SetGridIsEnabled(Player.Player1, false);
                    GameView.SetGridIsEnabled(Player.Player2, false);
                    break;
                default:
                    break;
            }
        }

        public void EnemyHit(Vector2 pos, out Player winner)
        {
            throw new NotImplementedException();
        }

        public void GameWon(Player player)
        {
            throw new NotImplementedException();
        }

        public void PlayerHit(Vector2 pos, out Player winner)
        {
            throw new NotImplementedException();
        }

        public void ProcessPlayerHit(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public void QuitGame(GameResult result)
        {
            throw new NotImplementedException();
        }
    }
}
