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
        public const string SettingsFilePath = "settings.json";

        public MainMenu MainMenuView;
        public GameController GameController;

        private SettingsModel settings;

        public MainMenuController()
        {
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
            if (File.Exists(SettingsFilePath))
                settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(SettingsFilePath));
            else
                settings = new SettingsModel();

            SettingsView view = new SettingsView(this);
            view.Show();
        }

        public bool SaveSettings(out Exception exception)
        {
            try {
                File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(settings));
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
