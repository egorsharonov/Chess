using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    class Move
    {
        PieceMoving pm;
        private readonly Board board;

        public Move(Board board)
        {
            this.board = board;
            pm = new PieceMoving("Pe2e4");
        }

        public bool CanMove(PieceMoving pm)
        {
            this.pm = pm;
            return
                CanMoveFrom() &&
                CanMoveTo() &&
                CanPieceMove();
        }

        bool CanMoveFrom()
        {
            return pm.from.OnBoard() &&
                   board.GetPieceAt(pm.from) == pm.piece &&
                   pm.piece.GetColor() == board.MoveColor;
        }
        bool CanMoveTo()
        {
            return pm.to.OnBoard() &&
                   pm.from != pm.to &&
                   board.GetPieceAt(pm.to).GetColor() != board.MoveColor;
        }
        bool CanPieceMove()
        {
            switch (pm.piece)
            {
                case Piece.whiteKing:
                    return CanKingMove();
                case Piece.whiteQueen:
                    return CanStraightMove();
                case Piece.whiteRook:
                    return (pm.SignX == 0 || pm.SignY == 0) &&
                            CanStraightMove();
                case Piece.whiteBishop:
                    return (pm.SignX != 0 && pm.SignY != 0) && CanStraightMove();
                case Piece.whiteKnight:
                    return CanKnightMove();
                case Piece.whitePawn:
                    return CanPawnMove();
                case Piece.blackKing:
                    return CanKingMove();
                case Piece.blackQueen:
                    return CanStraightMove();
                case Piece.blackRook:
                    return (pm.SignX == 0 || pm.SignY == 0) &&
                            CanStraightMove();
                case Piece.blackBishop:
                    return (pm.SignX != 0 && pm.SignY != 0) && CanStraightMove();
                case Piece.blackKnight:
                    return CanKnightMove();
                case Piece.blackPawn:
                    return CanPawnMove();
                default: return false;
            }
        }
        private bool CanCastle()
        {
            Color color = pm.piece.GetColor();
            string castling = board.fen.Split()[2];
            if (color == Color.white)
            {
                if (pm.to == new Square("c1") &&
                    board.GetPieceAt(new Square("d1")) == Piece.none &&
                    board.GetPieceAt(new Square("c1")) == Piece.none &&
                    board.GetPieceAt(new Square("b1")) == Piece.none &&
                    castling.Contains('Q'))
                    return true;
                if (pm.to == new Square("g1") &&
                    board.GetPieceAt(new Square("f1")) == Piece.none &&
                    board.GetPieceAt(new Square("g1")) == Piece.none &&
                    castling.Contains('K'))
                    return true;
            }
            else
            {

                if (pm.to == new Square("c8") &&
                    board.GetPieceAt(new Square("d8")) == Piece.none &&
                    board.GetPieceAt(new Square("c8")) == Piece.none &&
                    board.GetPieceAt(new Square("b8")) == Piece.none &&
                    castling.Contains('q'))
                    return true;
                if (pm.to == new Square("g8") &&
                    board.GetPieceAt(new Square("f8")) == Piece.none &&
                    board.GetPieceAt(new Square("g8")) == Piece.none &&
                    castling.Contains('k'))
                    return true;
            }
            return false;
        }
        private bool CanKingMove()
        {
            if ((pm.AbsDeltaX <= 1 && pm.AbsDeltaY <= 1) || CanCastle())
                return true;
            return false;
        }
        private bool CanKnightMove()
        {
            return (pm.AbsDeltaX == 1 && pm.AbsDeltaY == 2) ||
                   (pm.AbsDeltaX == 2 && pm.AbsDeltaY == 1);
        }
        private bool CanStraightMove()
        {
            Square at = pm.from;
            do
            {
                at = new Square(at.X + pm.SignX, at.Y + pm.SignY);
                if (at == pm.to)
                    return true;
            } while (at.OnBoard() &&
                     board.GetPieceAt(at) == Piece.none);
            return false;
        }
        private bool CanPawnMove()
        {
            if (pm.from.Y < 1 || pm.from.Y > 6)
                return false;
            int stepY = pm.piece.GetColor() == Color.white ? 1 : -1;
            return
                CanPawnGo(stepY) ||
                CanPawnJump(stepY) ||
                CanPawnEat(stepY) ||
                CanPawnEnPassant();
        }

        private bool CanPawnEnPassant()
        {
            string enPassant = board.fen.Split()[3];
            Square sq = new Square(enPassant);

            if (pm.to.Name != enPassant ||
               Math.Abs(pm.from.X - sq.X) != 1)
                return false;

            return (pm.piece == Piece.whitePawn && pm.from.Y == 4) ||
                   (pm.piece == Piece.blackPawn && pm.from.Y == 3);
        }
        private bool CanPawnGo(int StepY)
        {
            if (board.GetPieceAt(pm.to) == Piece.none)
                if (pm.DeltaX == 0)
                    if (pm.DeltaY == StepY)
                        return true;
            return false;
        }
        private bool CanPawnJump(int StepY)
        {
            if (board.GetPieceAt(pm.to) == Piece.none)
                if (pm.DeltaX == 0)
                    if (pm.DeltaY == 2 * StepY)
                        if (pm.from.Y == 1 || pm.from.Y == 6)
                            if (board.GetPieceAt(new Square(pm.from.X, pm.from.Y + StepY)) == Piece.none)
                                return true;
            return false;
        }
        private bool CanPawnEat(int StepY)
        {
            if (board.GetPieceAt(pm.to) != Piece.none)
                if (pm.AbsDeltaX == 1)
                    if (pm.DeltaY == StepY)
                        return true;
            return false;
        }
    }
}
