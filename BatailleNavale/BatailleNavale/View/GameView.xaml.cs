using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BatailleNavale.Controller;
using BatailleNavale.Model;

namespace BatailleNavale.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private IGameController controller;

        private static BitmapImage hitImage;
        private Vector2 updatingBoat;

        /// <summary>Contains all drawn boats on the UI (both grids)</summary>
        private List<UIBoat> DrawnBoats;

        static GameWindow()
        {
            hitImage = new BitmapImage(new Uri("./Resources/Hit.png", UriKind.RelativeOrAbsolute));
        }

        private List<Rectangle> playerGridBackground;
        private List<Rectangle> enemyGridBackground;

        public GameWindow(SingleplayerGameController controller_)
        {
            controller = controller_;

            InitializeComponent();

            DrawnBoats = new List<UIBoat>();
            playerGridBackground = new List<Rectangle>();
            enemyGridBackground = new List<Rectangle>();

            //Generate filler rectangles so drag and drop works.
            //PlayerGrid
            for (int x = 0; x < GridModel.SizeX; x++) {
                for (int y = 0; y < GridModel.SizeY; y++) {
                    Rectangle filler = new Rectangle
                    {
                        Fill = Brushes.Transparent,
                        Stretch = Stretch.Fill
                    };

                    PlayerGrid.Children.Add(filler);

                    Grid.SetColumn(filler, x);
                    Grid.SetRow(filler, y);
                    Panel.SetZIndex(filler, -100);

                    playerGridBackground.Add(filler);
                }
            }

            //EnemyGrid
            for (int x = 0; x < GridModel.SizeX; x++) {
                for (int y = 0; y < GridModel.SizeY; y++) {
                    Rectangle filler = new Rectangle
                    {
                        Fill = Brushes.Transparent,
                        Stretch = Stretch.Fill,
                    };

                    EnemyGrid.Children.Add(filler);

                    Grid.SetColumn(filler, x);
                    Grid.SetRow(filler, y);
                    Panel.SetZIndex(filler, -100);

                    filler.MouseDown += Cell_Clicked;

                    enemyGridBackground.Add(filler);
                }
            }
        }

        public void DrawRectangle(Rectangle rect, Vector2 pos, Player playerGrid)
        {

            if (playerGrid == Player.Player1)
                PlayerGrid.Children.Add(rect);
            else
                EnemyGrid.Children.Add(rect);
        }

        /// <summary>
        /// Display a boat on the UI
        /// </summary>
        /// <param name="boat"></param>
        public void DisplayBoat(BoatModel boat, Player playerGrid)
        {
            ImageBrush imageBrush = new ImageBrush(BoatModel.GetBoatImage(boat.BoatTypeId));
            imageBrush.Stretch = Stretch.UniformToFill;

            Rectangle boatRect = new Rectangle
            {
                //Fill = imageBrush,
                Fill = Brushes.Black,

            };

            Panel.SetZIndex(boatRect, 1);

            if (playerGrid == Player.Player1) {
                boatRect.MouseMove += Boat_MouseMove;

                PlayerGrid.Children.Add(boatRect);
            } else {
                EnemyGrid.Children.Add(boatRect);

                boatRect.MouseDown += Cell_Clicked;
            }


            Grid.SetColumn(boatRect, (int)boat.Position.X);
            Grid.SetRow(boatRect, (int)boat.Position.Y);

            if (boat.Size > 1) {
                if (boat.Orientation_ == BoatModel.Orientation.Horizontal)
                    Grid.SetColumnSpan(boatRect, boat.Size);
                else
                    Grid.SetRowSpan(boatRect, boat.Size);
            }

            DrawnBoats.Add(new UIBoat(boat, boatRect, playerGrid));
        }

        public void DisplayHit(Vector2 pos, Player playerGrid)
        {
            Rectangle hitRect = new Rectangle
            {
                Fill = new ImageBrush(hitImage),
                Stretch = Stretch.Fill
            };

            Grid.SetColumn(hitRect, (int)pos.X);
            Grid.SetRow(hitRect, (int)pos.Y);
            Panel.SetZIndex(hitRect, 10);

            DrawRectangle(hitRect, pos, playerGrid);
        }

        public void SetGridIsEnabled(Player playerGrid, bool enabled)
        {
            if (playerGrid == Player.Player1) {
                foreach (Rectangle rect in playerGridBackground) {
                    if (enabled)
                        rect.Fill = Brushes.Transparent;
                    else
                        rect.Fill = Brushes.LightGray;
                }
            } else {
                foreach (Rectangle rect in enemyGridBackground) {
                    if (enabled)
                        rect.Fill = Brushes.Transparent;
                    else
                        rect.Fill = Brushes.LightGray;
                }
            }
        }

        public void SetllBoatsForPlayerVisibility(bool visible, Player player = Player.Player2)
        {
            foreach (UIBoat boat in DrawnBoats) {
                if (boat.Player == player) {
                    if (visible)
                        boat.BoatUIElement.Visibility = Visibility.Visible;
                    else
                        boat.BoatUIElement.Visibility = Visibility.Hidden;
                }
            } 
        }

        private void Cell_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (controller.GameState != GameState.Player1Turn)
                return;

            Rectangle element = e.Source as Rectangle;

            int columnIndex = Grid.GetColumn(element);
            int rowIndex = Grid.GetRow(element);

            Console.WriteLine($"X: {columnIndex} Y: {rowIndex}; ColSpan: {Grid.GetColumnSpan(element)} RowSpan: {Grid.GetRowSpan(element)}");

            controller.ProcessPlayerHit(new Vector2(columnIndex, rowIndex));
        }

        public void RemoveBoat(BoatModel boat, Player playerGrid) //Warning: 2 boats on each grid with the same position will probably crash this
        {
            foreach (UIBoat item in DrawnBoats) {
                if (item.Boat == boat && item.Player == playerGrid) {
                    if (item.Player == Player.Player1)
                        PlayerGrid.Children.Remove(item.BoatUIElement);
                    else
                        EnemyGrid.Children.Remove(item.BoatUIElement);

                    DrawnBoats.Remove(item);
                    return;
                }
            }

            throw new Exception("Boat not found in specified grid."); //Not found
        }

        public struct UIBoat
        {
            public BoatModel Boat;
            public Rectangle BoatUIElement;
            public Player Player;

            public UIBoat(BoatModel boat, Rectangle rect, Player playerSide = Player.Player1)
            {
                Boat = boat;
                BoatUIElement = rect;
                Player = playerSide;
            }

            public void UpdatePosition(Vector2 pos)
            {
                Boat.Position = pos;
                BoatUIElement.SetValue(Grid.ColumnProperty, (int)pos.X);
                BoatUIElement.SetValue(Grid.RowProperty, (int)pos.Y);
            }
        }

        private void ReadyBtn_Click(object sender, RoutedEventArgs e)
        {
            controller.SetReady();
        }

        private void PlayerGrid_Drop(object sender, DragEventArgs e)
        {
            if (controller.GameState != GameState.PlayersChooseBoatsLayout)
                return;

            if (e.Handled)
                return;

            if (!(e.Source is Rectangle rectangle)) //Dropped on nothing
                return;

            Console.WriteLine($"PlayerGrid_Drop: {Grid.GetColumn(rectangle)} {Grid.GetRow(rectangle)}");

            int columnIndex = Grid.GetColumn(rectangle);
            int rowIndex = Grid.GetRow(rectangle);

            foreach (UIBoat boat in DrawnBoats) {
                if (boat.Boat.Position == new Vector2(columnIndex, rowIndex)) //User dropped the boat on another boat
                    return;
            }

            foreach (UIBoat boat in DrawnBoats) {
                if (boat.Boat.Position == updatingBoat) {
                    boat.UpdatePosition(new Vector2(columnIndex, rowIndex));
                }
            }

            e.Handled = true;
        }

        private void Boat_MouseMove(object sender, MouseEventArgs e)
        {
            if (controller.GameState != GameState.PlayersChooseBoatsLayout)
                return;

            if (!e.Handled && e.LeftButton == MouseButtonState.Pressed) {
                if (sender.GetType() != typeof(Rectangle)) {
                    return;
                }

                Rectangle element = e.Source as Rectangle;

                int columnIndex = Grid.GetColumn(element);
                int rowIndex = Grid.GetRow(element);

                Console.WriteLine($"Boat_MouseMove: {Grid.GetColumn(element)} {Grid.GetRow(element)}");

                updatingBoat = new Vector2(columnIndex, rowIndex);

                DragDrop.DoDragDrop(element, element, DragDropEffects.Move);

                e.Handled = true;
            }
        }
    }
}
