﻿using System;
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
        public bool RemoteIsReady;

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

            RemoteIsReady = false;

            MainMenuController = mainMenuController;
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();

            DurationSW = new Stopwatch();

            GameView = new GameWindow(this);
            GameView.Show();

            GameView.WriteInChat($"Hosting on {new IPEndPoint(IPAddress.Any, MainMenuController.UserDataModel.Port)} with protocol {NetworkCommunicator.PROTOCOL_TYPE}. UPNP: {MainMenuController.UserDataModel.UseUPnP}");
        }

        private void NetCom_ChatMessageReceivedEvent(string content)
        {
            GameView.WriteInChat(content, NetCom.RemotePlayer.PlayerData.Username);
        }

        private void NetCom_IllegalHitEvent(Hit hit)
        {
            throw new NotImplementedException();
        }

        private void NetCom_EnemyHitEvent(Hit hit)
        {
            GameView.RemoveHit(hit.Position, Player.Player2);
            EnemyGrid.Hits.Add(hit);
        }

        private void NetCom_GameEndedEvent(MessagesData.GameEndedData data)
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
            GameView.WriteInChat($"Player \"{NetCom.RemotePlayer.PlayerData.Username}\" joined.");
        }

        public void SetReady()
        {
            if (RemoteIsReady) {
                NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.GameReady, null);

                NetCom.Send(message);
                DurationSW.Start();
                ChangeGameState(GameState.Player1Turn);
            } else {
                GameView.WriteInChat("The remote player isn't ready yet !");
            }
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
            MessagesData.GameEndedData data = new MessagesData.GameEndedData
            {
                EnemyBoats = PlayerGrid.Boats
            };

            if (player == Player.Player1)
                data.Result = GameResult.EnemyWon; //From host point of view !
            else
                data.Result = GameResult.LocalPlayerWon;

            NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.GameEnded, data);
            NetCom.Send(message);
        }

        public void PlayerHit(Vector2 pos, out Player winner)
        {
            winner = Player.None;

            if (EnemyGrid.HitExists(pos))
                throw new Exception();

            Hit hit = new Hit
            {
                CurrentDateTime = DateTime.Now,
                Position = pos
            };

            EnemyGrid.Hits.Add(hit);

            if (IsGameWon(out Player player)) {
                winner = player;
                GameWon(player);
            }
        }

        public bool IsGameWon(out Player who)
        {
            bool gameWon = false;
            who = Player.None;

            int playerDestroyedBoats = PlayerGrid.GetDestroyedBoats().Count;
            int enemyDestroyedBoats = EnemyGrid.GetDestroyedBoats().Count;

            Console.WriteLine($"playerDestroyed: {playerDestroyedBoats}/{PlayerGrid.Boats.Count} enemyDestroyed: {enemyDestroyedBoats}/{EnemyGrid.Boats.Count}");

            if (playerDestroyedBoats == PlayerGrid.Boats.Count) {
                who = Player.Player2;
                gameWon = true;
            }

            if (enemyDestroyedBoats == EnemyGrid.Boats.Count) {
                who = Player.Player1;
                gameWon = true;
            }

            return gameWon;
        }

        public void ProcessPlayerHit(Vector2 pos)
        {
            PlayerHit(pos, out _);
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
            } else {
                NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Disconnect, null);
                NetCom.Send(message);
            }

            MainMenuController.SetInGame(false);
            MainMenuController.MainMenuView.Activate();
            GameView.Close();
        }
    }
}
