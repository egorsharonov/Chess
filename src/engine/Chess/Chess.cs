
using System.Collections.Generic;

namespace Library
{
    public class Chess
    {
        public string fen;
        private readonly Board board;
        private readonly Move moves;
        List<PieceMoving> allMoves;
        public bool IsCheckmate { get; private set; }
        public bool IsStalemate { get; private set; }
        public Chess(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            this.fen = fen;
            board = new Board(fen);
            moves = new Move(board);
            allMoves = new List<PieceMoving>();
        }

        Chess(Board board)
        {
            this.board = board;
            fen = board.fen;
            moves = new Move(board);
            allMoves = new List<PieceMoving>();
        }

        public Chess ChessMove(string move)
        {
            PieceMoving pm = new PieceMoving(move);
            if (!moves.CanMove(pm))
                return this;
            if (board.IsCheckAfterMove(pm))
                return this;
            Board nextBoard = board.Move(pm);
            Chess nextChess = new Chess(nextBoard);

            bool isCheck = nextChess.IsCheck();
            bool isEndGame = nextChess.GetAllMoves().Count == 0;
            nextChess.IsCheckmate = isCheck && isEndGame;
            nextChess.IsStalemate = !isCheck && isEndGame;
            return nextChess;
        }

        public char GetPieceAt(int x, int y)
        {
            Square square = new Square(x, y);
            Piece piece = board.GetPieceAt(square);
            return (piece == Piece.none) ? '.' : (char)piece;
        }

        void FindAllMoves()
        {
            allMoves = new List<PieceMoving>();
            foreach (PieceOnSquare ps in board.YieldPieces())
                foreach (Square to in Square.YieldSquares())
                {
                    PieceMoving pm = new PieceMoving(ps, to);
                    if (moves.CanMove(pm))
                        if (!board.IsCheckAfterMove(pm))
                            allMoves.Add(pm);
                }
        }
        public List<string> GetAllMoves()
        {
            FindAllMoves();
            List<string> list = new List<string>();
            foreach (PieceMoving pm in allMoves)
                list.Add(pm.ToString());
            return list;
        }
        public bool IsCheck() => board.IsCheck();
    }
}