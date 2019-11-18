using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;
using BatailleNavale.Model;
using System.IO;
using System.Windows;

namespace BatailleNavale.Controller
{
   public class MainMenuController
    {
        public const string UserDataFilePath = "user.json";

        public MainMenuWindow MainMenuView;
        public IGameController GameController;

        public UserDataModel UserDataModel;

        public MainMenuController()
        {
            if (File.Exists(UserDataFilePath))
                try {
                    UserDataModel = UserDataLoader.Load(UserDataFilePath);
                } catch (Exception) {
                    MessageBox.Show("An error occured while loading the user data.");
                    ResetSettings();
                }
            else {
                UserDataModel = new UserDataModel();
            }

            MainMenuView = new MainMenuWindow(this);

            MainMenuView.Show();
        }

        public void NewGame(GameSettings settings)
        {
            if (settings.GameMode == GameMode.Singleplayer) {
                SingleplayerGameController gameController = new SingleplayerGameController(settings.Difficulty);
                GameController = gameController;

                gameController.GenerateBoats();
                gameController.IAController.GenerateBoats(settings.BoatCount);
            } else {

            }
        }

        /// <summary>
        /// Reset the user settings/data.
        /// </summary>
        /// <param name="fileExists">If true, will act like the user is using this app for the first time.</param>
        /// <param name="resetAllSettings">If true, will also reset all game stats.</param>
        public void ResetSettings(bool fileExists = false, bool resetAllSettings = false)
        {
            UserDataModel = new UserDataModel();

            if (fileExists) {
                ShowSettings();
            }
        }

        public void ShowSettings()
        {
            SettingsWindow view = new SettingsWindow(this);
            view.Show();
        }

        public bool SaveSettings(out Exception exception)
        {
            try {
                UserDataLoader.Save(UserDataFilePath, UserDataModel);
            } catch (Exception ex) {
                exception = ex;
                return false;
            }

            exception = null;
            return true;
        }

        public void Close()
        {
            SaveSettings(out _);
            Application.Current.Shutdown(0);
        }

        public class GameSettings
        {
            public GameMode GameMode;
            public IAModel.Difficulty Difficulty = IAModel.Difficulty.None;

            /// <summary>Amount of boat to generate</summary>
            public int BoatCount = 5;
        }

        public enum GameMode
        {
            Singleplayer,
            Multiplayer
        }
    }
}
