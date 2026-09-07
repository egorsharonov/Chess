using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Text.Json;
using ChessApi.Models;

namespace ChessApi.Controllers
{
    public class gamesController : ApiController
    {
        private ModelChessDB db = new ModelChessDB();

        // GET: api/games
        public string GetGames()
        {
            Logic logic= new Logic();
            Game game = logic.GetGames();
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(game, options);
        }

        // GET: api/games/5/black
        public string GetGames(int GameID, string name)
        {
            Logic logic = new Logic();
            CurrentInfo game = logic.GetInfo(GameID,name);
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(game, options);
        }

        // GET: api/games/5/black/Pe2e4
        public string GetMove(int id, string name, string move)
        {
            Logic logic = new Logic();
            CurrentInfo game = logic.MakeMove(id, name, move);
            //game.Fen += " " + move;
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(game, options);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        private bool GameExists(int id)
        {
            return db.Games.Count(e => e.ID == id) > 0;
        }
    }
}