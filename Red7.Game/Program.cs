using Colorful;
using Red7.ConsoleManager;
using Red7.Core;
using Red7.Core.Helpers;
using Red7.Core.Infrastructure;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace Red7Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var red7Game = new Red7Game();

            Red7ConsoleManager.InitializeConsoleSetup(red7Game);
            Red7ConsoleManager.InitializeConsoleGame(red7Game.Players.Count);
            Red7ConsoleManager.DrawBoards(red7Game);

            Console.ReadLine();
        }

    }
}
