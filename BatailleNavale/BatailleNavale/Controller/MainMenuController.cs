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

        private bool inGame = false;

        public MainMenuController()
        {
            if (File.Exists(UserDataFilePath)) {
                try {
                    UserDataModel = UserDataLoader.Load(UserDataFilePath);
                } catch (Exception) {
                    MessageBox.Show("An error occured while loading the user data.");
                    ResetSettings();
                }
            } else {
                UserDataModel = new UserDataModel();
            }

            MainMenuView = new MainMenuWindow(this);

            MainMenuView.Show();
            RefreshStats();
        }

        public void NewGame(GameSettings settings)
        {
            if (settings.GameMode == GameMode.Singleplayer) {
                SingleplayerGameController gameController = new SingleplayerGameController(this, settings.Difficulty);
                GameController = gameController;

                gameController.GenerateBoats();
                gameController.IAController.GenerateBoats(settings.BoatCount);
            } else {
                //TODO: Make a MultiplayerClientGameController and MultiplayerHostGameController.
            }

            SetInGame(true);
        }

        public void SetInGame(bool inGame_)
        {
            inGame = inGame_;

            MainMenuView.SingleplayerBtn.IsEnabled = !inGame;
            MainMenuView.MultiplayerBtn.IsEnabled = false; //!inGame; //Always disabled until a MPGameController is created.

            RefreshStats();
        }

        public void RegisterGame(GameData game)
        {
            UserDataModel.Games.Add(game);
            RefreshStats();
        }

        public void RefreshStats()
        {
            Console.WriteLine("a " + UserDataModel.Games.Count);

            string totalPlayTime;
            if (UserDataModel.GetTotalPlayTime().TotalHours > 1d)
                totalPlayTime = $"Total play time: {UserDataModel.GetTotalPlayTime().Hours}h";
            else
                totalPlayTime = $"Total play time: {UserDataModel.GetTotalPlayTime().Minutes}m";

            MainMenuView.PlayerInfo.Content =
                $"{UserDataModel.Username}" + Environment.NewLine +
                $"Wins: {UserDataModel.GetWins()}" + Environment.NewLine +
                $"Loses: {UserDataModel.GetLoses()}" + Environment.NewLine +
                totalPlayTime + Environment.NewLine +
                $"Best score: {UserDataModel.GetBestScore()}";
        }

        /// <summary>
        /// Reset the user settings/data.
        /// </summary>
        /// <param name="fileExists">If true, will act like the user is using this app for the first time.</param>
        /// <param name="resetAllSettings">If true, will also reset all game stats.</param>
        public void ResetSettings(bool fileExists = false, bool resetAllSettings = false)
        {
            if (resetAllSettings)
                UserDataModel = new UserDataModel();
            else {
                UserDataModel.ResetStats();
            }

            SaveSettings(out _);

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
            if (!SaveSettings(out _)) {
                MessageBoxResult result = MessageBox.Show("An error occured while saving the user data, continue ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.No)
                    return;
            }

            Application.Current.Shutdown(0);
        }

        public class GameSettings
        {
            public GameMode GameMode;
            public IAModel.Difficulty Difficulty = IAModel.Difficulty.None;

            /// <summary>Amount of boat to generate</summary>
            public int BoatCount = 5;
        }
    }
}
