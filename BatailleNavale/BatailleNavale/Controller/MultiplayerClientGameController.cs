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
            NetCom.HitResultEvent += NetCom_HitResultEvent;
            NetCom.ChatMessageReceivedEvent += NetCom_ChatMessageReceivedEvent;

            MainMenuController = mainMenuController;
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();

            DurationSW = new Stopwatch();


            GameView = new GameWindow(this);
            GameView.Show();

            GameView.EnemyInfo.Content = string.Empty;
            GameView.WriteInChat($"Connecting to a remote client...");

            foreach (BoatModel boat in GenerateBoats()) {
                CreateBoat(Player.Player1, boat.Position, boat.Size, boat.Orientation_, boat.BoatTypeId);
            }
        }

        /// <summary>
        /// Create a new boat and display it on the UI.
        /// </summary>
        /// <param name="playerTeam"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        /// <param name="name"></param>
        public void CreateBoat(Player playerTeam, Vector2 pos, int size, BoatModel.Orientation orientation, int boatTypeId = -1)
        {
            BoatModel boat = new BoatModel(pos, size, orientation, boatTypeId);

            if (playerTeam == Player.Player1)
                PlayerGrid.Boats.Add(boat);
            else
                EnemyGrid.Boats.Add(boat);

            GameView.DisplayBoat(boat, playerTeam);
        }

        public List<BoatModel> GenerateBoats()
        {
            List<Vector2> usedPositions = new List<Vector2>();
            Random rnd = new Random();
            List<BoatPreset> boatPresets = BoatModel.GetBoatPresets();
            List<BoatModel> boats = new List<BoatModel>();

            for (int i = 0; i < boatPresets.Count; i++) {
                BoatPreset boatPreset = boatPresets[i];
                Vector2 pos = Vector2.Zero;
                BoatModel.Orientation orientation = (BoatModel.Orientation)rnd.Next(2);
                int X = rnd.Next(GridModel.SizeX);
                int Y = rnd.Next(GridModel.SizeY);

                do {
                    if (orientation == BoatModel.Orientation.Horizontal)
                        X = rnd.Next(GridModel.SizeX - boatPreset.boatSize);
                    else
                        Y = rnd.Next(GridModel.SizeY - boatPreset.boatSize);

                    pos = new Vector2(X, Y);
                } while (usedPositions.Contains(pos));

                for (int i2 = 0; i2 < boatPreset.boatSize; i2++) {
                    if (orientation == BoatModel.Orientation.Horizontal)
                        usedPositions.Add(new Vector2(pos.X + i2, pos.Y));
                    else
                        usedPositions.Add(new Vector2(pos.X, pos.Y + i2));
                }

                boats.Add(new BoatModel(pos, boatPreset.boatSize, orientation, boatPreset.boatId));
            }

            return boats;
        }

        private void NetCom_ChatMessageReceivedEvent(string content)
        {
            GameView.WriteInChat(content, NetCom.RemotePlayer.PlayerData.Username);
        }

        private void NetCom_HitResultEvent(MessagesData.HitResultData hitData)
        {
            if (hitData.IsIllegal) {
                //Fired if the the last hit we did is illegal.
                GameView.RemoveHit(hitData.hit.Position, Player.Player1);
                EnemyGrid.Hits.Remove(hitData.hit);
                GameView.WriteInChat($"Hit at position {hitData.hit.Position} was illegal and has been removed.");
            } else {
                GameView.DisplayHit(hitData.hit.Position, Player.Player2, hitData.BoatExists);
            }
        }

        private void NetCom_EnemyHitEvent(Hit hit)
        {
            GameView.RemoveHit(hit.Position, Player.Player2);
            EnemyGrid.Hits.Add(hit);
        }

        private void NetCom_PlayerReadyEvent(List<BoatModel> boats)
        {
            GameView.WriteInChat($"Player {NetCom.RemotePlayer.PlayerData.Username}");
        }

        private void NetCom_PlayerLeftEvent()
        {
            QuitGame(GameResult.Interupted);
        }

        public void SetReady()
        {
            DurationSW.Start();
            ChangeGameState(GameState.Player1Turn);

            NetCom.SetPlayerReady(PlayerGrid.Boats);
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

        public void ProcessPlayerHit(Vector2 pos)
        {
            if (GameState != GameState.GameEnded)
                return;

            NetCom.Hit(new Hit(pos));

            if (EnemyGrid.BoatExists(pos))
                GameView.DisplayHit(pos, Player.Player2, true); //Display a hitmarker where the player clicked.
            else
                GameView.DisplayHit(pos, Player.Player2, false);

            ChangeGameState(GameState.Player2Turn);
        }

        public void PlayerHit(Vector2 pos, out Player winner) //Host only.
        {
            throw new NotImplementedException();
        }

        public void EnemyHit(Vector2 pos, out Player winner) //Host only.
        {
            throw new NotImplementedException();
        }

        public void GameWon(Player player) //Host only.
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
