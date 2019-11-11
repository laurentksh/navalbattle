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
    public class GameController
    {
        public GameWindow GameView { get; private set; }

        public IAController IAController;

        public GridModel PlayerGrid;
        public GridModel EnemyGrid;

        public List<BoatModel> PlayerBoats = new List<BoatModel>();
        public List<BoatModel> EnemyBoats = new List<BoatModel>();

        public GameController(IAModel.Difficulty difficulty)
        {
            GameView = new GameWindow();
            GameView.Show();

            IAController = new IAController(this, difficulty);
        }

        /// <summary>
        /// Create a new boat and display it on the UI.
        /// </summary>
        /// <param name="playerTeam"></param>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <param name="orientation"></param>
        /// <param name="name"></param>
        public void CreateBoat(bool playerTeam, Vector2 pos, int size, BoatModel.Orientation orientation, string name = null)
        {
            BoatModel boat = new BoatModel(pos, size, orientation, name);

            if (playerTeam)
                PlayerBoats.Add(boat);
            else
                EnemyBoats.Add(boat);
        }

        public void GenerateBoats(int boatCount, bool playerTeam)
        {

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
