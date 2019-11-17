using BatailleNavale.Controller;
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
    public partial class MainMenu : Window
    {
        private MainMenuController controller;
        private bool gameSettingsDisplayed = false;
        private Storyboard storyboard = new Storyboard();

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

                DoubleAnimation da = new DoubleAnimation();
                da.From = 1.0;
                da.To = 0.3;
                da.RepeatBehavior = RepeatBehavior.Forever;
                da.AutoReverse = true;

                storyboard.Children.Add(da);
                Storyboard.SetTargetProperty(da, new PropertyPath("(Button.Opacity)"));
                Storyboard.SetTarget(da, SingleplayerBtn);
                storyboard.Begin();
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
            controller.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            controller.Close();
        }
    }
}
