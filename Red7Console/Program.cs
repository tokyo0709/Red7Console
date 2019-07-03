using Red7.Core;
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

            Console.WriteLine("Anthony's Board:\r\n");

            System.Console.WriteLine("Palette:");
            foreach (var item in red7Game.Players.Where(x => x.Id == 1).First().Palette.Cards)
            {
                
            }

            Console.WriteLine("1", Color.Red);
            Console.WriteLine("2", Color.Orange);
            Console.WriteLine("3", Color.Yellow);
            Console.WriteLine("4", Color.Green);
            Console.WriteLine("5", Color.LightBlue);
            Console.WriteLine("6", Color.Blue);
            Console.WriteLine("7", Color.Indigo);
            Console.ReadLine();
        }
    }
}
