using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;

namespace BatailleNavale.Controller
{
   public class MainMenuController
    {
        public MainMenu MainMenuView;
        public GameController GameController;

        public MainMenuController()
        {
            MainMenuView = new MainMenu(this);

            MainMenuView.Show();
        }

        public void NewGame(GameMode gameMode)
        {
            GameController = new GameController();
        }

        public enum GameMode
        {
            Singleplayer,
            Multiplayer
        }
    }
}
