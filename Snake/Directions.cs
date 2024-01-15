using System.Collections.Generic;

namespace snake_game
{
    public class Directions
    {
        //zmienna static jest zmienną dla całej klasy, nie tylko dla konkretnej instancji klasy
        public readonly static Directions Left = new Directions(0, -1);
        public readonly static Directions Right = new Directions(0, 1);
        public readonly static Directions Up = new Directions(-1, 0);
        public readonly static Directions Down = new Directions(1, 0);

        //zapis taki oznacza zmienną publiczną typu int tylko do odczytu
        public int Row_offset { get; }
        public int Col_offset { get; }

        //konstruktor(prywatny, ponieważ tylko metody wewnątrz tej klasy mogą zmieniać
        private Directions(int row_offset, int col_offset)
        {
            Row_offset = row_offset;
            Col_offset = col_offset;
        }

        public Directions Opposite()
        {
            return new Directions(-Row_offset, -Col_offset);
        }

        //Medota Equals() i GetHashCode() wygenerowana bezpośrednio w VisualStudio
        public override bool Equals(object obj)
        {
            return obj is Directions directions &&
                   Row_offset == directions.Row_offset &&
                   Col_offset == directions.Col_offset;
        }

        public override int GetHashCode()
        {
            int hashCode = -1382320372;
            hashCode = hashCode * -1521134295 + Row_offset.GetHashCode();
            hashCode = hashCode * -1521134295 + Col_offset.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Directions left, Directions right)
        {
            return EqualityComparer<Directions>.Default.Equals(left, right);
        }

        public static bool operator !=(Directions left, Directions right)
        {
            return !(left == right);
        }

    }
}
