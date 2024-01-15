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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace snake_game
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Jest to struktura danych słownik (dictionary), gdzie kluczem jest typ Grid_value, a wartością jest typ ImageSource. Słownik przechowuje mapowanie pomiędzy wartościami typu Grid_value a odpowiadającymi im obiektami ImageSource.
        private readonly Dictionary<Grid_value, ImageSource> grid_val_to_img = new Dictionary<Grid_value, ImageSource>
        {
            {Grid_value.Empty, Images.Empty },
            {Grid_value.Snake, Images.Body },
            {Grid_value.Food, Images.Food }
        };

        private readonly Dictionary<Directions, int> dir_to_rotation = new Dictionary<Directions, int>
        {
            { Directions.Up, 0 }, 
            { Directions.Right, 90 },
            { Directions.Down, 180 },
            { Directions.Left, 270 }
        };

        private readonly int rows = 15, cols = 15;
        private readonly Image[,] grid_images;
        private Game_state game_state;
        private bool game_running;
        public MainWindow()
        {
            InitializeComponent();
            grid_images = Setup_grid();
            game_state = new Game_state(rows, cols);
        }

        private async Task RunGame()
        {
            Draw();
            await Show_count_down();
            Overlay.Visibility = Visibility.Hidden;
            await Game_loop();
            await Show_Game_over();
            game_state = new Game_state(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //dopóki overlay jest widoczny Window_KeyDown nie włączy się
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!game_running)
            {
                game_running = true;
                await RunGame();
                game_running = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (game_state.Game_over)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    game_state.Change_direction(Directions.Left);
                    break;
                case Key.A:
                    game_state.Change_direction(Directions.Left);
                    break;
                case Key.Right:
                    game_state.Change_direction(Directions.Right);
                    break;
                case Key.D:
                    game_state.Change_direction(Directions.Right);
                    break;
                case Key.Up:
                    game_state.Change_direction(Directions.Up);
                    break;
                case Key.W:
                    game_state.Change_direction(Directions.Up);
                    break;
                case Key.Down:
                    game_state.Change_direction(Directions.Down);
                    break;
                case Key.S:
                    game_state.Change_direction(Directions.Down);
                    break;
            }
        }

        //asynchroniczna metoda nie blokuje innych wątków
        private async Task Game_loop()
        {
            while (!game_state.Game_over)
            {
                await Task.Delay(100);
                game_state.Move();
                Draw();
            }
        }

        private Image[,] Setup_grid()
        {
            Image[,] images = new Image[rows, cols];
            Game_grid.Rows = rows;
            Game_grid.Columns = cols;

            //Dla każdego pola ładujemy empty
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        //ponieważ obracanie głowy psuje animacje wykonuje rotacje względem środka punktu
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    Game_grid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw()
        {
            Draw_grid();
            Draw_snake_head();
            Score_text.Text = $"SCORE {game_state.Score}";
        }

        private void Draw_grid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Grid_value grid_val = game_state.Grid[r, c];
                    grid_images[r, c].Source = grid_val_to_img[grid_val];
                    //brak rotacji
                    grid_images[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void Draw_snake_head()
        {
            Position head_pos = game_state.Head_position();
            Image image = grid_images[head_pos.Row, head_pos.Col];
            image.Source = Images.Head;

            int rotation = dir_to_rotation[game_state.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task Draw_dead_snake()
        {
            List<Position> positions = new List<Position>(game_state.Snake_positions());

            for (int i = 0; i < positions.Count; i++) 
            { 
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                grid_images[pos.Row, pos.Col].Source = source;  
                await Task.Delay(50);
            }
        }

        private async Task Show_count_down()
        {
            for (int i = 3; i > 0; i--)
            {
                Overlay_text.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task Show_Game_over()
        {
            await Draw_dead_snake();
            Overlay.Visibility = Visibility.Visible;
            Overlay_text.Text = $"SCORE: {game_state.Score}";
            await Task.Delay(1000);
            Overlay_text.Text = "PRESS ANY KEY TO START";
        }
    }
}
