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

        public MainMenu MainMenuView;
        public GameController GameController;

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

            MainMenuView = new MainMenu(this);

            MainMenuView.Show();
        }

        public void NewGame(GameMode gameMode, IAModel.Difficulty difficulty = IAModel.Difficulty.None)
        {
            GameController = new GameController(difficulty);
            GameController.GenerateBoats(5, true);
            GameController.GenerateBoats(5, false);
        }

        /// <summary>
        /// Reset the user settings/data.
        /// </summary>
        /// <param name="fileExists">If true, will act like the user is using this app for the first time.</param>
        public void ResetSettings(bool fileExists = false)
        {
            UserDataModel = new UserDataModel();


        }

        public void ShowSettings()
        {
            SettingsView view = new SettingsView(this);
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

        public enum GameMode
        {
            Singleplayer,
            Multiplayer
        }
    }
}
