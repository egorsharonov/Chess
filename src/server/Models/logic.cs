using System.Collections.Generic;
using System.Linq;
using Library;
namespace ChessApi.Models
{
    public interface IRepository
    {
        CurrentInfo GetInfo(int gameID, string playerName);
        Game GetGame(int id);
        Game Create();

    }
    public class Logic : IRepository
    {
        private readonly ModelChessDB db;
        public Logic()
        {
            db = new ModelChessDB();
        }
        public Game GetGame(int id)
        {
            return db.Games.Find(id);
        }
        public Game GetGames()
        {
            return db.Games.OrderBy(g => g.ID).LastOrDefault();
        }
        public Player GetPlayer(int PlayerID)
        {
            return db.Players.Find(PlayerID);
        }
        public Player GetPlayer(string playerName)
        {
            return db.Players.Where(e => e.Name == playerName).FirstOrDefault();
        }
        public List<Player> GetPlayers(int id)
        {
            List<Player> list = new List<Player>();
            foreach (var side in db.Sides.Where(e => e.GameID == id)){
                Player p1 = GetPlayer(side.PlayerID);
                list.Add(p1);
            }
            return list;
        }
        public Side GetPlayerSide(int GameID, int PlayerID)
        {
            return db
                .Sides
                .Where(g => g.PlayerID == PlayerID)
                .Where(g => g.GameID == GameID)
                .FirstOrDefault();
        }
        public Side GetColorSide(int GameID, string color)
        {
            return db
                .Sides
                .Where(g => g.GameID == GameID)
                .Where(g => g.Color == color)
                .FirstOrDefault();
        }
        public Move GetLastMove(int GameID)
        {
            return db
                .Moves
                .Where(g => g.GameID == GameID)
                .OrderBy(g => g.PLY)
                .LastOrDefault();
        }
        public CurrentInfo GetInfo(int gameID, string playerName)
        {
            Game game = GetGame(gameID);
            string FEN = game.FEN;
            string Status = game.Status;
            int WhitePlayerID = GetColorSide(gameID, "w").PlayerID;
            int BlackPlayerID = GetColorSide(gameID, "b").PlayerID;
            string White = GetPlayer(WhitePlayerID).Name;
            string Black = GetPlayer(BlackPlayerID).Name;

            int playerID = GetPlayer(playerName).ID;
            Side side = GetPlayerSide(gameID, playerID);
            string LastMove = GetLastMove(gameID).MoveNext;
            string YourColor = side.Color;
            bool IsYourMove = GetLastMove(gameID).PlayerID == playerID;
            string OfferDraw = ( side.Draw == 1 ) ? "Yes" : "No";
            string Winner = "";
            if(game.Status == "done")
            {
                foreach (Player player in GetPlayers(gameID))
                {
                    if(GetPlayerSide(gameID, player.ID).Points==1)
                        Winner = player.Name;
                }
            }
            return new CurrentInfo()
            {
                GameID = gameID,
                FEN = FEN,
                Status = Status,
                White = White,
                Black = Black,
                LastMove = LastMove,
                YourColor = YourColor,
                IsYourMove = IsYourMove,
                OfferDraw = OfferDraw,
                Winner = Winner
            };
        }
        public Game Create()
        {
            Library.Chess chess = new Chess();
            Game game = new Game
            {
                FEN = chess.fen,
                Status = "play"
            };
            Player player1 = new Player
            {
                Name = "1",
                Password = "1"
            };
            Player player2 = new Player
            {
                Name = "2",
                Password = "2"
            };
            Side side1 = new Side
            {
                GameID = game.ID,
                PlayerID = player1.ID,
                Color = "w",
                Points = 0,
                Draw = 0,
                Resign = 0
            };
            Side side2 = new Side
            {
                GameID = game.ID,
                PlayerID = player2.ID,
                Color = "b",
                Points = 0,
                Draw = 0,
                Resign = 0
            };
            db.Games.Add(game);
            db.Sides.Add(side1);
            db.Sides.Add(side2);
            if(db.Players.Find(player1.ID) == null)
                db.Players.Add(player1);
            if(db.Players.Find(player2.ID) == null)
                db.Players.Add(player2);
            db.SaveChanges();
            return game;
        }
        public CurrentInfo MakeMove(int id,string name, string moveString)
        {
            Game game = GetGame(id);
            Move lastMove = GetLastMove(id);
            Side mySide, OtherSide;
            mySide = new Side();
            OtherSide = new Side();
            foreach (Player player in GetPlayers(id))
            {
                if(player.Name == name)
                    mySide = GetPlayerSide(id, player.ID);
                else
                    OtherSide = GetPlayerSide(id, player.ID);
            }
            CurrentInfo info = GetInfo(id, name);

            if (game == null) return info;
            if (game.Status != "play") return info;
            if (info.IsYourMove == false) return info;

            Chess chess = new Chess(game.FEN);
            Chess chessNext = chess.ChessMove(moveString);
            if (chessNext.fen == game.FEN) return info;
            

            game.FEN = chessNext.fen;
            if (chessNext.IsCheckmate || chess.IsStalemate)
            {
                game.Status = "done";
                mySide.Draw = chess.IsStalemate?1:0;
                mySide.Points = chess.IsStalemate ? (decimal)0.5 : 1;
            }
            db.Entry(game).State = System.Data.Entity.EntityState.Modified;
            db.Entry(mySide).State = System.Data.Entity.EntityState.Modified;
            db.Entry(OtherSide).State = System.Data.Entity.EntityState.Modified;
            Move move = new Move
            {
                GameID = id,
                PlayerID = GetPlayer(name).ID,
                PLY = lastMove.PLY += 1,
                FEN = chess.fen,
                MoveNext = moveString
            };
            db.Moves.Add(move);
            db.SaveChanges();
            info = GetInfo(id, name);
            return info;
        }
    }
}