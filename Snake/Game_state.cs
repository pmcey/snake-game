using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snake_game
{
    public class Game_state
    {
        public int Rows { get; }
        public int Cols { get; }
        public Grid_value[,] Grid { get; }
        public Directions Dir { get; private set; }
        public int Score { get; private set; }
        public bool Game_over { get; private set; }

        private readonly LinkedList<Directions> dir_changes = new LinkedList<Directions>();

        //Lista prywatna przechowująca obiekty typu 'Position'
        private readonly LinkedList<Position> snake_position = new LinkedList<Position>();
        private readonly Random random = new Random();

        //konstruktor inicjujący rozmiar obszaru gry
        public Game_state(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            //początkowo wszystkie pola są Grid_value.Empty ponieważ to pierwszy element na enum
            Grid = new Grid_value[rows, cols];
            Dir = Directions.Right;
            Add_snake();
            Add_food();
        }
        
        private void Add_snake()
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++) 
            {
                Grid[r, c] = Grid_value.Snake;
                snake_position.AddFirst(new Position(r, c));
            }
        }
        //metoda zwraca sekwencję obiektów typu Position, która może być iterowalna
        //metoda zwraca tylko puste pola
        private IEnumerable<Position> Empty_position()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == Grid_value.Empty)
                    {
                        //yield return zwraca wartość bez przerywania metody
                        yield return new Position(r, c);
                    }
                }
            }
        }
        private void Add_food()
        {
            List<Position> empty = new List<Position>(Empty_position());

            if (empty.Count <= 0)
            {
                return;
            }
            //random.Next(empty.Count) generuje losowy indeks od 0 do liczby elementów w liście empty minus 1. Następnie ten indeks jest używany do wybrania losowego elementu z listy empty.
            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = Grid_value.Food;
        }

        public Position Head_position()
        {
            return snake_position.First.Value;
        }

        public Position Tail_position()
        {
            return snake_position.Last.Value;
        }

        public IEnumerable<Position> Snake_positions()
        {
            return snake_position;
        }

        //pos - pozycja jedzonka
        private void Add_head(Position pos)
        {
            snake_position.AddFirst(pos);
            Grid[pos.Row, pos.Col] = Grid_value.Snake;
        }

        private void Remove_tail()
        {
            Position tail = Tail_position();
            Grid[tail.Row, tail.Col] = Grid_value.Empty;
            snake_position.RemoveLast();
        }

        private Directions Get_last_direction()
        {
            if (dir_changes.Count <= 0)
            {
                return Dir;
            }

            return dir_changes.Last.Value;
        }

        private bool Can_change_direction(Directions new_dir)
        {
            if (dir_changes.Count == 2)
            {
                return false;
            }
            Directions last_dir = Get_last_direction();
            return new_dir != last_dir && new_dir != last_dir.Opposite();
        }

        public void Change_direction(Directions dir)
        {
            //czy zmiana kierunku może być wykonana
            if (Can_change_direction(dir))
            {
                dir_changes.AddLast(dir);
            }
        }

        private bool Outside_grid(Position pos)
        {
            //zwraca True jeżeli warunek jest spełniony
            return pos.Row < 0 || pos.Col < 0 || pos.Row >= Rows || pos.Col >= Cols;
        }

        private Grid_value Will_hit(Position new_head_pos)
        {
            if (Outside_grid(new_head_pos))
            {
                return Grid_value.Outside;
            }

            //jeżeli ogon i głowa w tym samym miejscu
            if (new_head_pos == Tail_position())
            {
                return Grid_value.Empty;
            }

            return Grid[new_head_pos.Row, new_head_pos.Col];
        }

        public void Move()
        {
            if (dir_changes.Count > 0)
            {
                Dir = dir_changes.First.Value;
                dir_changes.RemoveFirst();
            }

            Position new_head_pos = Head_position().Translate(Dir);
            Grid_value hit = Will_hit(new_head_pos);

            if (hit == Grid_value.Outside || hit == Grid_value.Snake)
            {
                Game_over = true;
            }
            else if (hit == Grid_value.Empty)
            {
                Remove_tail();
                Add_head(new_head_pos);
            }
            else if (hit == Grid_value.Food)
            {
                Add_head(new_head_pos);
                Score++;
                Add_food();
            }
        }

    }
}
