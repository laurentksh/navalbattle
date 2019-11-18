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

        private Vector2 updatingBoat;

        /// <summary>Contains all drawn boats on the UI (both grids)</summary>
        private List<UIBoat> DrawnBoats = new List<UIBoat>();



        public GameWindow(SingleplayerGameController controller_)
        {
            controller = controller_;

            InitializeComponent();

            //Generate filler rectangles so drag and drop works.
            for (int x = 0; x < GridModel.SizeX; x++) {
                for (int y = 0; y < GridModel.SizeY; y++) {
                    Rectangle filler = new Rectangle
                    {
                        Fill = Brushes.LightGray,
                        Stretch = Stretch.Fill
                    };

                    PlayerGrid.Children.Add(filler);

                    Grid.SetColumn(filler, x);
                    Grid.SetRow(filler, y);
                    Panel.SetZIndex(filler, -100);
                }
            }
        }

        /// <summary>
        /// Display a boat on the UI
        /// </summary>
        /// <param name="boat"></param>
        public void DisplayBoat(BoatModel boat, bool playerGrid)
        {
            ImageBrush imageBrush = new ImageBrush(BoatModel.GetBoatImage(boat.BoatTypeId));
            imageBrush.Stretch = Stretch.UniformToFill;

            Rectangle boatRect = new Rectangle
            {
                //Fill = imageBrush,
                Fill = Brushes.Black,
                
            };

            Panel.SetZIndex(boatRect, 10);

            if (playerGrid) {
                boatRect.MouseMove += Boat_MouseMove;

                PlayerGrid.Children.Add(boatRect);
            } else
                EnemyGrid.Children.Add(boatRect);

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

        public void RemoveBoat(BoatModel boat, bool playerGrid) //Warning: 2 boats on each grid with the same position will probably crash this
        {
            foreach (UIBoat item in DrawnBoats) {
                if (item.Boat == boat && item.PlayerSide == playerGrid) {
                    if (item.PlayerSide)
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
            public bool PlayerSide;

            public UIBoat(BoatModel boat, Rectangle rect, bool playerSide = true)
            {
                Boat = boat;
                BoatUIElement = rect;
                PlayerSide = playerSide;
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

            Console.WriteLine(sender);

            if (!(e.Source is Rectangle rectangle)) //Dropped on nothing
                return;

            //Grid grid = VisualTreeHelper.GetParent(rectangle) as Grid;
            Console.WriteLine($"PlayerGrid_Drop: {Grid.GetColumn(rectangle)} {Grid.GetRow(rectangle)}");

            int columnIndex = Grid.GetColumn(rectangle);
            var rowIndex = Grid.GetRow(rectangle);

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
                    Console.WriteLine("Not a Rectangle");
                    return;
                }
                
                var element = e.Source as Rectangle;

                int columnIndex = Grid.GetColumn(element);
                var rowIndex = Grid.GetRow(element);

                Console.WriteLine($"Boat_MouseMove: {Grid.GetColumn(element)} {Grid.GetRow(element)}");

                updatingBoat = new Vector2(columnIndex, rowIndex);

                DragDrop.DoDragDrop(element, element, DragDropEffects.Move);

                e.Handled = true;
            }
        }
    }
}
