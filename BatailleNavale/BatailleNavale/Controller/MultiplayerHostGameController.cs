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
    public class MultiplayerHostGameController : IGameController
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

        public MultiplayerHostGameController(MainMenuController mainMenuController)
        {
            GameMode = GameMode.Multiplayer;
            Host = true;
            GameState = GameState.PlayersChooseBoatsLayout;
            Result = GameResult.Interupted;

            NetCom = new NetworkCommunicator();
            NetCom.PlayerJoinedEvent += NetCom_PlayerJoinedEvent;
            NetCom.PlayerLeftEvent += NetCom_PlayerLeftEvent;
            NetCom.PlayerReadyEvent += NetCom_PlayerReadyEvent;
            NetCom.GameEndedEvent += NetCom_GameEndedEvent;
            NetCom.EnemyHitEvent += NetCom_EnemyHitEvent;
            NetCom.IllegalHitEvent += NetCom_IllegalHitEvent;
            NetCom.ChatMessageReceivedEvent += NetCom_ChatMessageReceivedEvent;

            MainMenuController = mainMenuController;
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();

            DurationSW = new Stopwatch();

            GameView = new GameWindow(this);
            GameView.Show();
        }

        private void NetCom_ChatMessageReceivedEvent(string content)
        {
            
        }

        private void NetCom_IllegalHitEvent(Hit hit)
        {
            throw new NotImplementedException();
        }

        private void NetCom_EnemyHitEvent(Hit hit)
        {
            throw new NotImplementedException();
        }

        private void NetCom_GameEndedEvent(GameResult result)
        {
            throw new NotImplementedException();
        }

        private void NetCom_PlayerReadyEvent()
        {
            throw new NotImplementedException();
        }

        private void NetCom_PlayerLeftEvent()
        {
            throw new NotImplementedException();
        }

        private void NetCom_PlayerJoinedEvent()
        {
            throw new NotImplementedException();
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
