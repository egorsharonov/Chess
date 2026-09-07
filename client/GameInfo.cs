using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessClient
{
    public struct GameInfo
    {
        public int GameID;
        public string FEN;
        public string Status;
        public string White;
        public string Black;
        public string LastMove;
        public string YourColor;
        public bool IsYourMove;
        public string OfferDraw;
        public string Winner;

        public GameInfo(NameValueCollection list)
        {
            GameID = int.Parse(list["GameID"]);
            FEN = list["FEN"];
            Status = list["Status"];
            White = list["White"];
            Black = list["Black"];
            LastMove = list["LastMove"];
            YourColor = list["YourColor"];
            IsYourMove = bool.Parse(list["IsYourMove"]);
            OfferDraw = list["OfferDraw"];
            Winner = list["Winner"];
        }

        override
        public string ToString() =>
            "GameID = " + GameID +
            "\nFEN = " + FEN +
            "\nStatus = " + Status +
            "\nWhite = " + White +
            "\nBlack = " + Black +
            "\nLastMove = " + LastMove +
            "\nIsYourMove = " + IsYourMove +
            "\nOfferDraw = " + OfferDraw +
            "\nWinner = " + Winner;

    }
}
