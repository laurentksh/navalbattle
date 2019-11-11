using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;

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
            ModelBoat boat = new ModelBoat(x, y, size, orientation);

            boats.Add(boat);
        }

        public void GenerateGrid(int boatCount)
        {

        }
    }
}
