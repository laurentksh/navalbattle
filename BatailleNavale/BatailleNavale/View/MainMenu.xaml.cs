﻿using BatailleNavale.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatailleNavale.View
{
    /// <summary>
    /// Logique d'interaction pour MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        private MainMenuController controller;

        private bool gameSettingsDisplayed = false;

        public MainMenu(MainMenuController controller_)
        {
            controller = controller_;

            InitializeComponent();

            MultiplayerBtn.IsEnabled = false;
            GameSettingsGB.Visibility = Visibility.Hidden;

            foreach (string item in Enum.GetNames(typeof(Model.IAModel.Difficulty)))
                DifficultyCB.Items.Add(item);

            if (DifficultyCB.Items.Count > 0)
                DifficultyCB.SelectedIndex = 0;
        }

        private void SingleplayerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gameSettingsDisplayed) {
                gameSettingsDisplayed = false;
                GameSettingsGB.Visibility = Visibility.Hidden;
                controller.NewGame(MainMenuController.GameMode.Singleplayer, (Model.IAModel.Difficulty)DifficultyCB.SelectedIndex);
            } else {
                gameSettingsDisplayed = true;
                GameSettingsGB.Visibility = Visibility.Visible;
            }
        }

        private void MultiplayerBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.NewGame(MainMenuController.GameMode.Multiplayer);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.ShowSettings();
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
