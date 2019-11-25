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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatailleNavale.View
{
    /// <summary>
    /// Logique d'interaction pour SettingsView.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private MainMenuController controller;

        public SettingsWindow(MainMenuController controller_)
        {
            InitializeComponent();

            controller = controller_;

            UsernameTB.Text = controller.UserDataModel.Username;
            HostPortTB.Text = Convert.ToString(controller.UserDataModel.Port);
            EnableUPnPCB.IsChecked = controller.UserDataModel.UseUPnP;
        }
        
        private void SaveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.UserDataModel.Username = UsernameTB.Text;
            controller.UserDataModel.Port = Convert.ToInt32(HostPortTB.Text);

            if (controller.SaveSettings(out Exception ex)) { //Save operation successful
                MessageBox.Show("Settings saved successfully !", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            } else { //Save operation failed
                MessageBox.Show("An error occured while saving the settings: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure ? You will lose your statistics and your settings.", "Confirm action", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes) {
                controller.ResetSettings(false, true);
                MessageBox.Show("User data reset successfully ! The application will now restart.", "Success", MessageBoxButton.OK);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown(0);
            }
        }

        private void ChangeProfilePic_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Multiselect = false;
            dialog.DefaultExt = ".png";
            dialog.Filter = "PNG Files (*.png)|*.png|JPEG Files (*.jpeg)|*.jpeg|JPG Files (*.jpg)|*.jpg";

            bool? result = dialog.ShowDialog(this);

            if (result.HasValue && result.Value) {
                controller.ChangeProfilePicture(dialog.FileName);
            }
        }

        private void ResetProfilePic_Click(object sender, RoutedEventArgs e)
        {
            controller.UserDataModel.ResetProfilePicture();
            controller.RefreshUserData();
        }
    }
}
