using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using BatailleNavale.Controller;

namespace BatailleNavale.View
{
    /// <summary>
    /// Logique d'interaction pour MultiplayerView.xaml
    /// </summary>
    public partial class MultiplayerView : Window
    {
        public MainMenuController Controller;

        public MultiplayerView(MainMenuController controller)
        {
            Controller = controller;

            InitializeComponent();
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            Controller.HostGame();
            Close();
        }

        private void JoinBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IPAddress.TryParse(IPTB.Text, out IPAddress address)) {
                MessageBox.Show("Invalid IP address !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ushort.TryParse(PortTB.Text, out ushort port)) {
                MessageBox.Show("Invalid port number !", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Controller.JoinGame(new IPEndPoint(address, port));
            Close();
        }
    }
}
