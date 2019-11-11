using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;
using BatailleNavale.Model;
using System.IO;
using Newtonsoft.Json;

namespace BatailleNavale.Controller
{
   public class MainMenuController
    {
        public const string UserDataFilePath = "user.json";

        public MainMenu MainMenuView;
        public GameController GameController;

        public UserDataModel UserDataModel;

        public MainMenuController()
        {
            if (File.Exists(UserDataFilePath))
                UserDataModel = JsonConvert.DeserializeObject<UserDataModel>(File.ReadAllText(UserDataFilePath));
            else
                UserDataModel = new UserDataModel();

            MainMenuView = new MainMenu(this);

            MainMenuView.Show();
        }

        public void NewGame(GameMode gameMode, IAModel.Difficulty difficulty = IAModel.Difficulty.None)
        {
            GameController = new GameController(difficulty);
            GameController.GenerateBoats(5, true);
            GameController.GenerateBoats(5, false);
        }

        public void ShowSettings()
        {
            SettingsView view = new SettingsView(this);
            view.Show();
        }

        public bool SaveSettings(out Exception exception)
        {
            try {
                File.WriteAllText(UserDataFilePath, JsonConvert.SerializeObject(UserDataModel));
            } catch (Exception ex) {
                exception = ex;
                return false;
            }

            exception = null;
            return true;
        }

        public enum GameMode
        {
            Singleplayer,
            Multiplayer
        }
    }
}
