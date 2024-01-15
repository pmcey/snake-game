namespace snake_game
{
    public class Position
    {
        public int Row { get;  }
        public int Col { get;  }

        //konstruktor
        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public Position Translate(Directions dir)
        {
            return new Position(Row + dir.Row_offset, Col + dir.Col_offset);
        }
    }
}
