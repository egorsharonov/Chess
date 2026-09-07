using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    struct Square
    {
        public static Square none = new Square(-1, 1);
        public int X { get; private set; }
        public int Y { get; private set; }

        public Square(int x, int y)
        {
            if ( x==-1 && y==1) { X= -1; Y = 1; }
            else if (x >= 0 && y >= 0 && x < 8 && y < 8) { X = x; Y = y; }
            else { this = none; }
        }

        public Square(string e2)
        {
            if (e2.Length == 2 &&
                e2[0] >= 'a' && e2[0] <= 'h' &&
                e2[1] >= '1' && e2[1] <= '8')
            {
                X = e2[0] - 'a';
                Y = e2[1] - '1';
            }
            else
                this = none;
        }

        public bool OnBoard()
        {
            return X >= 0 && X < 8 &&
                    Y >= 0 && Y < 8;
        }

        public string Name { get { return ((char)('a' + X)).ToString() + (Y + 1).ToString(); } }

        public static bool operator ==(Square left, Square right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !GetType().Equals(obj.GetType()))
                return false;
            Square right = (Square)obj;
            return X == right.X && Y == right.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator !=(Square left, Square right)
        {
            return !(left == right);
        }
        public static IEnumerable<Square> YieldSquares()
        {
            for (int y = 0; y < 8; ++y)
                for (int x = 0; x < 8; ++x)
                    yield return new Square(x, y);
        }
    }
}
