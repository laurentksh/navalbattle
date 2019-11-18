using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;
using System.Numerics;
using BatailleNavale.Model;

namespace BatailleNavale.Controller
{
    public class SingleplayerGameController : IGameController
    {
        public GameState GameState { get; set; }

        public GameWindow GameView { get; set; }
        public IAController IAController;

        public GridModel PlayerGrid { get; set; }
        public GridModel EnemyGrid { get; set; }
        

        public List<BoatModel> PlayerBoats;
        public List<BoatModel> EnemyBoats;

        public SingleplayerGameController(IAModel.Difficulty difficulty)
        {
            GameView = new GameWindow(this);
            GameView.Show();

            GameState = GameState.PlayersChooseBoatsLayout;
            IAController = new IAController(this, difficulty);
            PlayerGrid = new GridModel();
            EnemyGrid = new GridModel();
            PlayerBoats = new List<BoatModel>();
            EnemyBoats = new List<BoatModel>();
        }

        /// <summary>
        /// Create a new boat and display it on the UI.
        /// </summary>
        /// <param name="playerTeam"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        /// <param name="name"></param>
        public void CreateBoat(bool playerTeam, Vector2 pos, int size, BoatModel.Orientation orientation)
        {
            BoatModel boat = new BoatModel(pos, size, orientation);

            if (playerTeam)
                PlayerBoats.Add(boat);
            else
                EnemyBoats.Add(boat);

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
            Vector2 pos;
            int size;
            BoatModel.Orientation orientation;
            BoatPresets boatPreset;
            List<BoatPresets> boatPresets = BoatModel.GetBoatPresets();

            for (int i = 0; i < boatPresets.Count; i++) { //Random boat generation for now, will change later.
                boatPreset = boatPresets[i];
                size = boatPreset.boatSize;
                orientation = (BoatModel.Orientation)rnd.Next(1);

                do {
                    pos = new Vector2(rnd.Next(GridModel.SizeX), rnd.Next(GridModel.SizeY));
                } while (usedPositions.Contains(pos));

                for (int i2 = 0; i2 < size; i2++) {
                    if (orientation == BoatModel.Orientation.Horizontal)
                        usedPositions.Add(new Vector2(pos.X + i2, pos.Y));
                    else
                        usedPositions.Add(new Vector2(pos.X, pos.Y + i2));
                }

                CreateBoat(true, pos, size, orientation);
            }
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        public void SetReady()
        {
            ChangeGameState(GameState.Player1Turn);
        }

        public void ChangeGameState(GameState state)
        {
            GameState = state;

            switch (GameState) {
                case GameState.PlayersChooseBoatsLayout:
                    GameView.SetGridIsEnabled(true, true);
                    GameView.SetGridIsEnabled(false, false);
                    break;
                case GameState.Player1Turn:
                    GameView.SetGridIsEnabled(true, false);
                    GameView.SetGridIsEnabled(false, true);
                    break;
                case GameState.Player2Turn:
                    GameView.SetGridIsEnabled(true, true);
                    GameView.SetGridIsEnabled(false, false);
                    break;
                case GameState.GameEnded:
                    GameView.SetGridIsEnabled(true, false);
                    GameView.SetGridIsEnabled(false, false);
                    break;
                default:
                    break;
            }
        }

        public async void ProcessPlayerHit(Vector2 pos)
        {
            try {
                PlayerHit(pos);
            } catch (Exception) {
                return;
            }

            GameView.DisplayHit(pos, true);

            Random rnd = new Random();

            await Task.Delay(rnd.Next(1000, 4000)); //Fake that the AI is processing the next hit.

            ProcessIAHit(IAController.GetNextTarget());
        }

        public async void ProcessIAHit(Vector2 pos)
        {
            try {
                EnemyHit(pos);
            } catch (Exception) {
                return;
            }

            GameView.DisplayHit(pos, false);

            await Task.Delay(2000);
        }

        /// <summary>
        /// Method used by the view to process hits from the player.
        /// </summary>
        /// <param name="pos"></param>
        public void PlayerHit(Vector2 pos)
        {
            if (EnemyGrid.HitExists(pos))
                throw new Exception();

            EnemyGrid.Hits.Add(pos);
        }

        /// <summary>
        /// Method used by the AI component to process hits from the AI.
        /// </summary>
        /// <param name="pos"></param>
        public void EnemyHit(Vector2 pos)
        {
            if (PlayerGrid.HitExists(pos))
                throw new Exception();

            PlayerGrid.Hits.Add(pos);
        }
    }
}
