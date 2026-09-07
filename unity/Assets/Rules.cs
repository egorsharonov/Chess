using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
//using static UnityEditor.Progress;

public class Rules : MonoBehaviour
{
    //const string HOST = "http://localhost:44383/api/";
    //string user;
    DragNDrop dad;
    Chess chess;
    //ChessClient client;
    // Start is called before the first frame update
    public Rules()
    {
        dad = new DragNDrop(); 
    }
    
    public void Start()
    {
        //user = SystemInfo.deviceUniqueIdentifier.Substring(0, 8);
        //client = new ChessClient.ChessClient(HOST, user);
        //ChessClient.GameInfo game = client.GetCurrentGame();
        chess = new Chess();

        ShowPieces();
        MarkValidPieces();

        InvokeRepeating("Refresh", 2, 2);
    }

    void Refresh()
    {
        if (!dad.IsPickedUp)
        {
            //chess = new Chess(client.GetCurrentGame().FEN);
            ShowPieces();
            MarkValidPieces();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dad.Action())
        {
            string from = GetSquare(dad.pickPosition);
            string to = GetSquare(dad.dropPosition);
            string piece = chess.GetPieceAt(from).ToString();
            string move = piece + from + to;
            Debug.Log(move);
            //chess = new Chess(client.SendMove(move).FEN);
            chess = chess.ChessMove(move); //
            ShowPieces();
            MarkValidPieces();
        }
        if (dad.IsPickedUp)
        {
            MarkValidMoves(dad.pickPosition);
        }
    }

    string GetSquare(Vector2 position)
    {
        int x = Convert.ToInt32(position.x / 2.56);
        int y = Convert.ToInt32(position.y / 2.56);
        return ((char)('a' + x)).ToString() + (y + 1).ToString();
    }

    void ShowPieces()
    {
        int nr = 0;
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; ++x) 
            {
                string piece = chess.GetPieceAt(x, y).ToString();
                if (piece == ".") continue;
                PlacePiece($"box ({nr})", piece, y, x );
                nr++;
            }
        for (; nr < 32; nr++)
            PlacePiece($"box ({nr})", "q", 9, 9);
    }
    void MarkValidPieces()
    {
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; ++x)
                MarkSquare(x, y, false);
        foreach (string moves in chess.GetAllMoves())
        {
            int x, y;
            GetCoord(moves.Substring(1, 2), out x, out y);
            MarkSquare(x, y, true);
        }

    }

    public void MarkValidMoves(Vector2 pickPosition)
    {
        string from = GetSquare(pickPosition);
        string piece = chess.GetPieceAt(from).ToString();
        for (int y = 0; y < 8; y++)
            for (int x = 0; x < 8; ++x)
                MarkSquare(x, y, false);
        foreach (string moves in chess.GetAllMoves())
        {
            if (moves.Substring(1,2) != from) continue;
            int x, y;
            string to = moves.Substring(3, 2);
            GetCoord(to, out x, out y);
            MarkSquare(x, y, true);
        }

    }
    public void GetCoord(string e2, out int x, out int y)
    {
        x = 9;
        y = 9;
        if (e2.Length == 2 &&
            e2[0] >= 'a' && e2[0] <= 'h' &&
            e2[1] >= '1' && e2[1] <= '8')
        {
            x = e2[0] - 'a';
            y = e2[1] - '1';
        }
    }
    void PlacePiece(string box, string piece, int x, int y)
    {
        GameObject goBox = GameObject.Find(box);
        GameObject goPiece = GameObject.Find(piece);
        GameObject goSquare = GameObject.Find("" + x + y);

        var spritePiece = goPiece.GetComponent<SpriteRenderer>();
        var spriteBox = goBox.GetComponent<SpriteRenderer>();
        spriteBox.sprite = spritePiece.sprite;
        goBox.transform.position = goSquare.transform.position;
            
    }
    void MarkSquare(int x, int y, bool isMarked)
    {
        GameObject goSquare = GameObject.Find("" + y + x);
        GameObject goCell;
        string color = (x + y) % 2 == 0 ? "Black" : "White";
        if (isMarked)
            goCell = GameObject.Find(color + "SquareMarked");
        else
            goCell = GameObject.Find(color + "Square");
        var spriteSquare = goSquare.GetComponent<SpriteRenderer>();
        var spriteCell = goCell.GetComponent<SpriteRenderer>();
        spriteSquare.sprite = spriteCell.sprite;
    }
}

class DragNDrop
{
    enum State
    {
        None,
        drag
    }
    public Vector2 pickPosition { get; private set; }
    public Vector2 dropPosition { get; private set; }
    public bool IsPickedUp {get; private set;} 
    Vector2 offset;
    State state;
    GameObject item;
    public DragNDrop()
    {
        state = State.None;
        item = null;
        IsPickedUp = false;
    }
    public bool Action()
    {
        switch (state)
        {
            case State.None:
                if (IsMouseButtonPressed())
                    PickUp();
                break;
            case State.drag:
                if (IsMouseButtonPressed())
                    Drag();
                else
                {
                    Drop();
                    return true;
                }
                break;
        }
        return false;
    }

    bool IsMouseButtonPressed()
    {
        return Input.GetMouseButton(0);
    }

    void PickUp()
    {
        Vector2 clickPosition = GetClickPosition();
        Transform clickedItem = GetItemAt(clickPosition);
        if (clickedItem == null) return;

        pickPosition = clickedItem.position;
        item = clickedItem.gameObject;
        state = State.drag;
        offset = pickPosition - clickPosition;
        IsPickedUp = true;  
    }
    Vector2 GetClickPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    Transform GetItemAt(Vector2 position)
    {
        RaycastHit2D[] pieces = Physics2D.RaycastAll(position, position, 0.5f);
        if(pieces.Length == 0 )
        {
            return null;
        }
        return pieces[0].transform;
    }

    void Drag()
    {
        item.transform.position = GetClickPosition();
    }

    void Drop()
    {
        dropPosition = item.transform.position;
        state = State.None;
        item = null;
        IsPickedUp = false;
    }
}

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
    public char GetPieceAt(string sq)
    {
        int x = sq[0] - 'a';
        int y = sq[1] - '1';
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
enum Color
{
    none,
    white,
    black
}

static class ColorMethods
{
    public static Color FlipColor(this Color color)
    {
        if (color == Color.black) return Color.white;
        if (color == Color.white) return Color.black;
        return Color.none;
    }
}

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
        return (char)piece + from.Name + to.Name + ((promotion != Piece.none) ? (char)promotion : "");
    }
}

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

struct Square
{
    public static Square none = new Square(-1, 1);
    public int X { get; private set; }
    public int Y { get; private set; }

    public Square(int x, int y)
    {
        if (x == -1 && y == 1) { X = -1; Y = 1; }
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

    public override bool Equals(object obj)
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

