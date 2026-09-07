using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Library
{
    class Board
    {
        public string fen { get; private set; }

        private readonly Piece[,] pieces;
        public Color MoveColor { get; private set; }
        public int MoveNumber { get; private set; }

        public Board(string fen)
        {
            this.fen = fen;
            pieces = new Piece[8, 8];
            Init();
        }

        void Init()
        {
            string[] parts = fen.Split();
            if (parts.Length != 6) return;
            InitPieces(parts[0]);
            MoveColor = (parts[1] == "b") ? Color.black : Color.white;
            MoveNumber = int.Parse(parts[5]);
        }

        void InitPieces(string data)
        {
            for (int j = 8; j >= 2; j--)
                data = data.Replace(j.ToString(), (j - 1).ToString() + "1");
            data = data.Replace('1', '.');
            string[] lines = data.Split('/');
            for (int y = 7; y >= 0; --y)
                for (int x = 0; x < 8; x++)
                    pieces[x, y] = lines[7 - y][x] == '.' ? Piece.none :
                        (Piece)lines[7 - y][x];
        }

        public IEnumerable<PieceOnSquare> YieldPieces()
        {
            foreach (Square square in Square.YieldSquares())
                if (GetPieceAt(square).GetColor() == MoveColor)
                    yield return new PieceOnSquare(GetPieceAt(square), square);
        }
        void GenerateFen(string partOfFen)
        {
            fen = $"{FenPieces()} {(MoveColor == Color.white ? "w" : "b")} {partOfFen} {MoveNumber}";

        }
        string CanCastle(PieceMoving pm)
        {
            string oldCastle = fen.Split()[2];
            if (oldCastle == "-") return oldCastle;
            string castle = "";
            if (oldCastle.Contains('K') && pm.piece != Piece.whiteKing && pm.from != new Square("h1"))
                castle += "K";
            if (oldCastle.Contains('Q') && pm.piece != Piece.whiteKing && pm.from != new Square("a1"))
                castle += "Q";
            if (oldCastle.Contains('Q') && pm.piece != Piece.whiteKing && pm.from != new Square("a1"))
                castle += "Q";
            if (oldCastle.Contains('k') && pm.piece != Piece.blackKing && pm.from != new Square("h8"))
                castle += "k";
            if (oldCastle.Contains('q') && pm.piece != Piece.blackKing && pm.from != new Square("a8"))
                castle += "q";
            if (castle == "") castle = "-";
            return castle;
        }
        string PassableField(PieceMoving pm)
        {
            Piece p1 = GetPieceAt(new Square(pm.to.X - 1, pm.to.Y));
            Piece p2 = GetPieceAt(new Square(pm.to.X + 1, pm.to.Y));
            if ((pm.piece == Piece.whitePawn && (p1 == Piece.blackPawn || p2 == Piece.blackPawn) && pm.to.Y - pm.from.Y == 2) ||
                (pm.piece == Piece.blackPawn && (p1 == Piece.whitePawn || p2 == Piece.whitePawn) && pm.from.Y - pm.to.Y == 2))
            {
                string field = "";
                field += (char)('a' + pm.from.X);
                field += (char)(pm.from.Y + '0');
                return field;
            }
            return "-";
        }
        string ScoreToDraw(PieceMoving pm)
        {
            string oldScoreToDraw = fen.Split()[4];
            bool isResetCounterToDraw = (pm.piece == Piece.blackPawn || pm.piece == Piece.whitePawn || GetPieceAt(pm.to) != Piece.none);
            string ScoreToDraw = isResetCounterToDraw ? "0" : Convert.ToString(Int32.Parse(oldScoreToDraw) + 1);
            return ScoreToDraw;
        }
        string FenPieces()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 7; y >= 0; --y)
            {
                for (int x = 0; x < 8; x++)
                    sb.Append(pieces[x, y] == Piece.none ? '1' : (char)pieces[x, y]);
                if (y > 0)
                    sb.Append('/');
            }
            string eight = "11111111";
            for (int j = 8; j >= 2; j--)
                sb.Replace(eight.Substring(0, j), j.ToString());
            return sb.ToString();
        }

        public Piece GetPieceAt(Square square)
        {
            if (square.OnBoard())
                return pieces[square.X, square.Y];
            return Piece.none;
        }
        void SetPieceAt(Square square, Piece piece)
        {
            if (square.OnBoard())
                pieces[square.X, square.Y] = piece;
        }
        public Board Move(PieceMoving pm)
        {
            Board next = new Board(fen);
            string partOfFen = CanCastle(pm) + " " + PassableField(pm) + " " + ScoreToDraw(pm);
            next.SetPieceAt(pm.from, Piece.none);
            next.SetPieceAt(pm.to, pm.promotion == Piece.none ? pm.piece : pm.promotion);
            additionalMove(next, pm);
            if (MoveColor == Color.black)
                next.MoveNumber++;
            next.MoveColor = MoveColor.FlipColor();
            next.GenerateFen(partOfFen);
            return next;
        }
        void additionalMove(Board next, PieceMoving pm)
        {
            CastleMove(next, pm);
            EnPassantMove(next, pm);
        }
        void CastleMove(Board next, PieceMoving pm)
        {
            if (pm.ToString() == "Ke1c1")
            {
                next.SetPieceAt(new Square("a1"), Piece.none);
                next.SetPieceAt(new Square("d1"), Piece.whiteRook);
            }
            if (pm.ToString() == "Ke1g1")
            {
                next.SetPieceAt(new Square("h1"), Piece.none);
                next.SetPieceAt(new Square("f1"), Piece.whiteRook);
            }
            if (pm.ToString() == "ke8c8")
            {
                next.SetPieceAt(new Square("a8"), Piece.none);
                next.SetPieceAt(new Square("d8"), Piece.blackRook);
            }
            if (pm.ToString() == "ke8g8")
            {
                next.SetPieceAt(new Square("h8"), Piece.none);
                next.SetPieceAt(new Square("f8"), Piece.blackRook);
            }
            
        }
        void EnPassantMove(Board next, PieceMoving pm)
        {
            Square enPassant = new Square(fen.Split()[3]);
            if (pm.to.Name == enPassant.Name && pm.piece == Piece.whitePawn)
            {
                next.SetPieceAt(new Square(enPassant.X, enPassant.Y - 1), Piece.none);
            }
            if (pm.to.Name == enPassant.Name && pm.piece == Piece.blackPawn)
            {
                next.SetPieceAt(new Square(enPassant.X, enPassant.Y + 1), Piece.none);
            }
        }
        bool CanEatKing()
        {
            Square badKing = FindBadKing();
            Move moves = new Move(this);
            foreach (PieceOnSquare ps in YieldPieces())
            {
                PieceMoving pm = new PieceMoving(ps, badKing);
                if (moves.CanMove(pm))
                    return true;
            }
            return false;
        }

        private Square FindBadKing()
        {
            Piece badKing = ((MoveColor == Color.black) ? Piece.whiteKing : Piece.blackKing);
            foreach (Square square in Square.YieldSquares())
            {
                if (GetPieceAt(square) == badKing)
                    return square;
            }
            return Square.none;
        }
        public bool IsCheck()
        {
            Board after = new Board(fen)
            {
                MoveColor = MoveColor.FlipColor()
            };
            return after.CanEatKing();
        }
        public bool IsCheckAfterMove(PieceMoving pm)
        {
            Board after = Move(pm);
            return after.CanEatKing();
        }
    }
}
