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

            GameView.EnemyInfo.Content = string.Empty;
            GameView.WriteInChat($"Connecting to a remote client...");
        }

        private void NetCom_ChatMessageReceivedEvent(string content)
        {
            GameView.WriteInChat(content, NetCom.RemotePlayer.PlayerData.Username);
        }

        private void NetCom_IllegalHitEvent(Hit hit)
        {
            //Fired if the the last hit we did is illegal.
            GameView.RemoveHit(hit.Position, Player.Player1);
            EnemyGrid.Hits.Remove(hit);
            GameView.WriteInChat($"Hit at position {hit.Position} was illegal and has been removed.");
        }

        private void NetCom_EnemyHitEvent(Hit hit)
        {
            GameView.RemoveHit(hit.Position, Player.Player2);
            EnemyGrid.Hits.Add(hit);
        }

        private void NetCom_PlayerReadyEvent()
        {
            throw new NotImplementedException();
        }

        private void NetCom_PlayerLeftEvent()
        {
            QuitGame(GameResult.Interupted);
        }

        public void SetReady()
        {
            DurationSW.Start();
            ChangeGameState(GameState.Player1Turn);

            NetCom.SetPlayerReady();
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

                if (NetCom.Connected) {
                    NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Disconnect, null);
                    NetCom.Send(message);
                }
            } else {
                if (NetCom.Connected) {
                    NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Disconnect, null);
                    NetCom.Send(message);
                }
            }

            MainMenuController.SetInGame(false);
            MainMenuController.MainMenuView.Activate();
            GameView.Close();
        }

        private void NetCom_GameEndedEvent(MessagesData.GameEndedData data)
        {
            GameView.SetAllBoatsForPlayerVisibility(true, Player.Player2);

            if (data.Result == GameResult.EnemyWon) {
                Result = GameResult.EnemyWon;
                GameView.GameWonLbl.Content = $"{NetCom.RemotePlayer.PlayerData.Username} won !";
            } else {
                Result = GameResult.LocalPlayerWon;
                GameView.GameWonLbl.Content = $"{MainMenuController.UserDataModel.Username} won !";
            }

            GameView.GameWonLbl.Visibility = System.Windows.Visibility.Visible;
            GameView.QuitBtn.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
