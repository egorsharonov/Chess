using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    class PieceMoving
    {
        public Piece piece { get; private set; }
        public Square from { get; private set; }
        public Square to { get; private set; }
        public Piece promotion { get; private set; }
        public PieceMoving(PieceOnSquare pieceSquare, Square to, Piece promotion = Piece.none)
        {
            piece = pieceSquare.piece;
            from = pieceSquare.square;
            this.to = to;
            this.promotion = promotion;
        }
        public PieceMoving(string move)
        {
            piece = (Piece)move[0];
            from = new Square(move.Substring(1, 2));
            to = new Square(move.Substring(3, 2));
            promotion = (move.Length == 6) ? (Piece)move[5] : Piece.none;
        }
        public int DeltaX { get { return to.X - from.X; } }
        public int DeltaY { get { return to.Y - from.Y; } }
        public int AbsDeltaX { get { return Math.Abs(DeltaX); } }
        public int AbsDeltaY { get { return Math.Abs(DeltaY); } }
        public int SignX { get { return Math.Sign(DeltaX); } }
        public int SignY { get { return Math.Sign(DeltaY); } }

        public override string ToString()
        {
            return (char)piece + from.Name + to.Name + ((promotion != Piece.none) ? ((char)promotion).ToString() : "");
        }
    }
}
