using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    enum Piece
    {
        none,

        whiteKing = 'K',
        whiteQueen = 'Q',
        whiteRook = 'R',
        whiteBishop = 'B',
        whiteKnight = 'N',
        whitePawn = 'P',

        blackKing = 'k',
        blackQueen = 'q',
        blackRook = 'r',
        blackBishop = 'b',
        blackKnight = 'n',
        blackPawn = 'p',
    }

    static class PieceMethods
    {
        public static Color GetColor(this Piece piece)
        {
            if (piece == Piece.none)
                return Color.none;
            return (piece == Piece.whiteKing ||
                piece == Piece.whiteQueen ||
                piece == Piece.whiteRook ||
                piece == Piece.whiteBishop ||
                piece == Piece.whiteKnight ||
                piece == Piece.whitePawn)
              ? Color.white : Color.black;
        }
    }
}
