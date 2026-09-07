using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    class PieceOnSquare
    {
        public Piece piece { get; private set; }
        public Square square { get; private set; }
        public PieceOnSquare(Piece piece, Square square)
        {
            this.piece = piece;
            this.square = square;
        }
    }
}
