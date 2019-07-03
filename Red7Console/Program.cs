using Red7.Core;
using Red7.Core.Helpers;
using Red7.Core.Infrastructure;
using System;
using System.Drawing;
using System.Linq;
using Console = Colorful.Console;

namespace Red7Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var red7Game = new Red7Game();
            red7Game.AddPlayer(new Player(1, "Anthony"));
            red7Game.AddPlayer(new Player(2, "Callen"));
            red7Game.AddPlayer(new Player(3, "Rose"));
            red7Game.BeginGame();

            if (red7Game.Players.Where(x => x.ActivePlayer == true).Count() > 1) throw new Exception("There should not be more than one active player.");
                        
            var activePlayer = red7Game.Players.Where(x => x.ActivePlayer == true).First();

            foreach (var player in red7Game.Players)
            {
                Console.WriteLine($"{player.Name}'s Board:\r\n", Color.LightCyan);

                Console.WriteLine("Palette:", Color.White);
                foreach (var item in player.Palette.Cards)
                {
                    Console.Write("|", Color.White);
                    Console.Write($"{item.Value}", item.Color.GetConsoleColor());
                    Console.Write("| ", Color.White);
                }

                Console.WriteLine("\r\n\r\nHand:", Color.White);
                foreach (var item in player.Hand.Cards)
                {
                    Console.Write("|", Color.White);
                    if (player.Id != activePlayer.Id)
                    {
                        Console.Write("X", Color.White);
                    }
                    else
                    {
                        Console.Write($"{item.Value}", item.Color.GetConsoleColor());
                    }
                    Console.Write("| ", Color.White);
                }

                Console.WriteLine("\r\n");
            }

            Console.WriteLine("Canvas:", Color.LightGray);
            Console.Write("|", Color.White);
            Console.Write($"{red7Game.Canvas.GetActiveCanvasCard().Value}", red7Game.Canvas.GetActiveCanvasCard().Color.GetConsoleColor());
            Console.Write("|", Color.White);

            
            Console.WriteLine($"\r\n\r\n{red7Game.Players.Where(x => x.ActivePlayer == true).First().Name}'s turn.", Color.White);

            Console.ReadLine();
        }
    }
}
