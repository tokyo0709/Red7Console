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
            red7Game.BeginGame();

            foreach (var player in red7Game.Players)
            {
                Console.WriteLine($"{player.Name}'s Board:\r\n", Color.White);

                Console.WriteLine("Palette:", Color.White);
                foreach (var item in player.Palette.Cards)
                {
                    Console.Write($"{item.Value} ", item.Color.GetConsoleColor());
                }

                Console.WriteLine("\r\n\r\nHand:", Color.White);
                foreach (var item in player.Hand.Cards)
                {
                    Console.Write($"{item.Value} ", item.Color.GetConsoleColor());
                }

                Console.WriteLine("\r\n");
            }
            
            Console.ReadLine();
        }
    }
}
