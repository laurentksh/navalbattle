using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatailleNavale.View;
using BatailleNavale.Model;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using BatailleNavale.Net;

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
                    ResetSettings(true, true);
                }
            } else {
                UserDataModel = new UserDataModel();
            }

            MainMenuView = new MainMenuWindow(this);

            MainMenuView.Show();
            RefreshUserData();
        }

        public void Play(GameSettings settings)
        {
            if (settings.GameMode == GameMode.Singleplayer) {
                SingleplayerGameController gameController = new SingleplayerGameController(this, settings.Difficulty);
                GameController = gameController;

                gameController.GenerateBoats();
                gameController.IAController.GenerateBoats(settings.BoatCount);
                gameController.GameView.SetAllBoatsForPlayerVisibility(false, Player.Player2);
            } else {
                MultiplayerView mpView = new MultiplayerView(this);
                mpView.Show();
            }

            SetInGame(true);
        }

        public async void HostGame()
        {
            try {
                MultiplayerHostGameController controller = new MultiplayerHostGameController(this);
                await controller.NetCom.CreateServer(UserDataModel.Port, NetworkCommunicator.PROTOCOL_TYPE, UserDataModel.UseUPnP);
            } catch (Open.Nat.NatDeviceNotFoundException ex) {
                MessageBox.Show("Could not connect to the NAT device. If you keep getting this error, try the following steps:" + Environment.NewLine +
                    "1. Connect to another network." + Environment.NewLine +
                    "2. Make sure your NAT device is connected properly." + Environment.NewLine +
                    "3. Disable UPnP in the settings and create a static rule directly in your router.");
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex);
            }
        }

        public async void JoinGame(IPEndPoint host)
        {
            try {
                MultiplayerClientGameController controller = new MultiplayerClientGameController(this);
                await controller.NetCom.Connect(host, NetworkCommunicator.PROTOCOL_TYPE, new SimpleUserDataModel(UserDataModel));
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex);
            }
        }

        public void SetInGame(bool inGame_)
        {
            inGame = inGame_;

            MainMenuView.SingleplayerBtn.IsEnabled = !inGame;
            MainMenuView.MultiplayerBtn.IsEnabled = !inGame;

            RefreshUserData();
        }

        public void RegisterGame(GameData game)
        {
            UserDataModel.Games.Add(game);
            RefreshUserData();
        }

        public void RefreshUserData()
        {
            DisplayProfilePicture();

            MainMenuView.PlayerInfo.Content = UserDataModel.ToString();
        }

        public void DisplayProfilePicture()
        {
            BitmapImage pic = new BitmapImage();
            pic.BeginInit();
            pic.StreamSource = new MemoryStream(Convert.FromBase64String(UserDataModel.ProfilePicture));
            pic.EndInit();

            MainMenuView.PlayerPicture.Source = pic;
        }

        public void ChangeProfilePicture(string path)
        {
            Image img = Image.FromFile(path);

            Image resizedImg = ResizeImage(img, 96, 96);
            img.Dispose();

            UserDataModel.SetProfilePicture(resizedImg);
            RefreshUserData();

            resizedImg.Dispose();
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="newWidth">The width to resize to.</param>
        /// <param name="newHeight">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        private static Image ResizeImage(Image image, int newWidth, int newHeight) //TODO: Fix (Doesn't do shit ?)
        {
            try {
                var destRect = new Rectangle(0, 0, newWidth, newHeight);
                var destImage = new Bitmap(newWidth, newHeight);

                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage)) {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes()) {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                return destImage;
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return null;
        }

        /// <summary>
        /// Reset the user settings/data.
        /// </summary>
        /// <param name="showWelcomeWindow">If true, will act like the user is using this app for the first time.</param>
        /// <param name="resetAllSettings">If true, will also reset all game stats.</param>
        public void ResetSettings(bool showWelcomeWindow = false, bool resetAllSettings = false)
        {
            if (resetAllSettings)
                UserDataModel = new UserDataModel();
            else
                UserDataModel.ResetStats();

            SaveSettings(out _);

            if (showWelcomeWindow) {
                ShowSettings(); //Temp
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

            RefreshUserData();
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
