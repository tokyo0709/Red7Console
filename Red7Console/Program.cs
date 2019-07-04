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
            red7Game.AddPlayer(new Player(1, "Anthony"));
            red7Game.AddPlayer(new Player(2, "Callen"));
            red7Game.AddPlayer(new Player(3, "Rose"));
            red7Game.AddPlayer(new Player(4, "Luca"));
            red7Game.BeginGame();

            if (red7Game.Players.Where(x => x.ActivePlayer == true).Count() > 1) throw new Exception("There should not be more than one active player.");


            var root = GetApplicationRoot();
            string newPath = Path.GetFullPath(Path.Combine(root, @"..\"));

            FigletFont font = FigletFont.Load($"{newPath}/Red7.Console/FigletFonts/standard.flf");
            Figlet figlet = new Figlet(font);

            Console.WriteLine(figlet.ToAscii("Red Seven"), Color.Red);
            Console.ReadLine();

            Red7ConsoleManager.InitializeConsole(red7Game.Players.Count);
            Red7ConsoleManager.DrawBoards(red7Game);

            Console.ReadLine();
        }
        
        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

    }
}
