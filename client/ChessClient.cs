using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessClient
{
    public class ChessClient
    {
        public string host { get; private set; }
        public string user { get; private set; }

        int CurrentGameID;

        public ChessClient(string host, string user)
        {
            this.host = host;   
            this.user = user;   
        }
        public GameInfo GetCurrentGame()
        {
            GameInfo game = new GameInfo(ParseJson(CallServer()));
            CurrentGameID = game.GameID;
            return new GameInfo();
        }

        public GameInfo SendMove(string move) 
        {
            string json = CallServer(CurrentGameID + "/" + move);
            var list = ParseJson(json);
            GameInfo game = new GameInfo(list);
            return game;
        }

        private string CallServer(string param = "")
        {
            WebRequest request = WebRequest.Create(host + user + "/" + param);  
            WebResponse response = request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();  
        }
        private NameValueCollection ParseJson(string json)
        {
            NameValueCollection list = new NameValueCollection();
            string pattern = @"(\w+)\: ?([^,}]*)?";
            RegexOptions options = RegexOptions.Multiline;
            foreach (Match m in Regex.Matches(json, pattern, options))
            {
                Console.WriteLine("'{0}' found at index {1}.", m.Value, m.Index);
            }
            return list;    
        }
    }
}
