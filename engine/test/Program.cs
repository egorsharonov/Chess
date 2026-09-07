using Library;
using System;
using System.Collections.Generic;
namespace test
{
    class Program
    {
        static void Main()
        {
            Random random = new();
            Chess chess = new();
            List<string> list;
            while (true)
            {
                list = chess.GetAllMoves();
                Console.WriteLine(chess.fen);
                Console.WriteLine(ChessToAscii(chess));
                Console.WriteLine(chess.IsCheck() ? "CHECK" : "-");
                string output = chess.IsCheckmate ? "IsCheckmate" : "-";
                output = chess.IsStalemate ? "IsCheckmate" : output;
                if(output != "-")
                {
                    Console.WriteLine(output);
                    break;
                }
                foreach (string moves in list) 
                    Console.Write(moves + "\t");
                Console.WriteLine();
                Console.Write("> ");
                string move = Console.ReadLine() ?? "";
                if (move == "q") break;
                if (move == "") move = list[random.Next(list.Count)];
                chess = chess.ChessMove(move);
            }
        }

        static string ChessToAscii(Chess chess)
        {
            string text = "  +-----------------+\n";
            for(int y = 7; y >= 0; y--)
            {
                text += y + 1;
                text += " | ";
                for(int x = 0; x < 8; x++)
                {
                    text += chess.GetPieceAt(x, y) + " ";
                }
                text += "|\n";
            }
            text += "  +-----------------+\n";
            text += "    a b c d e f g h\n";
            return text;
        }
    }
}
