using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Numerics;
using BatailleNavale.Model;
using BatailleNavale.View;
using System.Diagnostics;

namespace BatailleNavale.Controller
{
    public class SingleplayerGameController : IGameController
    {
        public GameMode GameMode { get; set; }
        public bool? Host { get; set; }
        public GameState GameState { get; set; }

        public MainMenuController MainMenuController { get; set; }

        public GameWindow GameView { get; set; }
        public IAController IAController;

        public GridModel PlayerGrid { get; set; }
        public GridModel EnemyGrid { get; set; }

        public GameResult Result { get; set; }

        private Stopwatch DurationSW;

        public SingleplayerGameController(MainMenuController mainMenuController, IAModel.Difficulty difficulty)
        {
            GameMode = GameMode.Singleplayer;
            Host = null;
            GameState = GameState.PlayersChooseBoatsLayout;
            Result = GameResult.Interupted;

            MainMenuController = mainMenuController;
            IAController = new IAController(this, difficulty);
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();

            DurationSW = new Stopwatch();

            
            GameView = new GameWindow(this);
            GameView.Show();

            GameView.SetAllBoatsForPlayerVisibility(false, Player.Player2);
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

        /// <summary>
        /// Generate boats for the local player.
        /// </summary>
        /// <param name="boatCount"></param>
        public void GenerateBoats()
        {
            List<Vector2> usedPositions = new List<Vector2>();
            Random rnd = new Random();
            List<BoatPreset> boatPresets = BoatModel.GetBoatPresets();

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

                CreateBoat(Player.Player1, pos, boatPreset.boatSize, orientation, boatPreset.boatId);
            }
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void SetReady()
        {
            DurationSW.Start();

            ChangeGameState(GameState.Player1Turn);
        }

        public void ChangeGameState(GameState state)
        {
            GameState = state;
            Console.WriteLine(state);
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

        public async void ProcessPlayerHit(Vector2 pos)
        {
            Player winner = Player.None;

            try {
                PlayerHit(pos, out winner);
            } catch (Exception) {
                return;
            }

            if (winner != Player.None)
                return;

            GameView.DisplayHit(pos, Player.Player2); //Display a hitmarker where the player clicked.

            GameView.SetGridIsEnabled(Player.Player1, true);
            GameView.SetGridIsEnabled(Player.Player2, false);

            Random rnd = new Random();

            await Task.Delay(rnd.Next(1000, 4000)); //Fake that the AI is processing the next hit.

            ChangeGameState(GameState.Player2Turn);

            ProcessIAHit(IAController.GetNextTarget());
        }

        public async void ProcessIAHit(Vector2 pos)
        {
            Player winner = Player.None;

            try {
                EnemyHit(pos, out winner);
            } catch (Exception) {
                return;
            }

            if (winner != Player.None)
                return;

            GameView.DisplayHit(pos, Player.Player1);

            await Task.Delay(2000);

            ChangeGameState(GameState.Player1Turn);
        }

        public void GameWon(Player player)
        {
            ChangeGameState(GameState.GameEnded);
            GameView.SetAllBoatsForPlayerVisibility(true, Player.Player2);

            GameView.GameWonLbl.Content = $"{player} won !";
            GameView.GameWonLbl.Visibility = System.Windows.Visibility.Visible;

            if (player == Player.Player1)
                Result = GameResult.LocalPlayerWon;
            else
                Result = GameResult.EnemyWon;

            GameView.QuitBtn.Visibility = System.Windows.Visibility.Visible;
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

        /// <summary>
        /// Method used by the view to process hits from the player.
        /// </summary>
        /// <param name="pos"></param>
        public void PlayerHit(Vector2 pos, out Player won)
        {
            won = Player.None;

            if (EnemyGrid.HitExists(pos))
                throw new Exception();

            Hit hit = new Hit
            {
                CurrentDateTime = DateTime.Now,
                Position = pos
            };

            EnemyGrid.Hits.Add(hit);

            if (IsGameWon(out Player player)) {
                won = player;
                GameWon(player);
            }
        }

        /// <summary>
        /// Method used by the AI component to process hits from the AI.
        /// </summary>
        /// <param name="pos"></param>
        public void EnemyHit(Vector2 pos, out Player won)
        {
            won = Player.None;

            if (PlayerGrid.HitExists(pos))
                throw new Exception();

            Hit hit = new Hit
            {
                CurrentDateTime = DateTime.Now,
                Position = pos
            };

            PlayerGrid.Hits.Add(hit);

            if (IsGameWon(out Player player)) {
                won = player;
                GameWon(player);
            }
        }
    }
}
