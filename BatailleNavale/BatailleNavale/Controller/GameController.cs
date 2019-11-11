using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;
using System.Numerics;

namespace BatailleNavale.Controller
{
    public class GameController
    {
        public GameWindow GameView { get; private set; }

        public ModelGrid PlayerGrid;
        public ModelGrid EnemyGrid;

        public List<ModelBoat> boats = new List<ModelBoat>();

        public GameController()
        {
            GameView = new GameWindow();

            GameView.Show();
        }

        public void CreateBoat (int x, int y, int size, ModelBoat.Orientation orientation)
        {
            ModelBoat boat = new ModelBoat(x, y, size, orientation, null);

            boats.Add(boat);
        }

        public void GenerateGrid(int boatCount)
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
