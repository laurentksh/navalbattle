using BatailleNavale.Controller;
using BatailleNavale.Model;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatailleNavale.View
{
    /// <summary>
    /// Logique d'interaction pour MainMenu.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        private MainMenuController controller;
        private bool gameSettingsDisplayed = false;
        private Storyboard storyboard = new Storyboard();

        public MainMenuWindow(MainMenuController controller_)
        {
            controller = controller_;

            InitializeComponent();

            MultiplayerBtn.IsEnabled = false;
            GameSettingsGB.Visibility = Visibility.Hidden;

            foreach (string item in Enum.GetNames(typeof(IAModel.Difficulty)))
                DifficultyCB.Items.Add(item);

            if (DifficultyCB.Items.Count > 0)
                DifficultyCB.SelectedIndex = 0;

            //Button blink animation (TODO: Improve)
            DoubleAnimation da = new DoubleAnimation();
            da.From = 1.0;
            da.To = 0.3;
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.AutoReverse = true;

            storyboard.Children.Add(da);
            Storyboard.SetTargetProperty(da, new PropertyPath("(Button.Opacity)"));
            Storyboard.SetTarget(da, SingleplayerBtn);
        }

        private void SingleplayerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gameSettingsDisplayed) {
                gameSettingsDisplayed = false;
                GameSettingsGB.Visibility = Visibility.Hidden;
                storyboard.Stop();

                MainMenuController.GameSettings settings = new MainMenuController.GameSettings
                {
                    BoatCount = 5,
                    Difficulty = (IAModel.Difficulty)DifficultyCB.SelectedIndex,
                    GameMode = GameMode.Singleplayer
                };

                controller.NewGame(settings);
            } else {
                gameSettingsDisplayed = true;
                GameSettingsGB.Visibility = Visibility.Visible;

                storyboard.Begin();
            }
        }

        private void MultiplayerBtn_Click(object sender, RoutedEventArgs e)
        {
            MainMenuController.GameSettings settings = new MainMenuController.GameSettings
            {
                BoatCount = 5,
                GameMode = GameMode.Multiplayer,
                Difficulty = IAModel.Difficulty.None
            };

            controller.NewGame(settings);
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.ShowSettings();
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            controller.Close();
        }
    }
}
