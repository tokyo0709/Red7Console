using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Console = Colorful.Console;

namespace Red7.ConsoleManager.Helpers
{
    public static class ConsoleHelper
    {
        public static void DrawBoxedWord(int left, int top, string word, Color color)
        {
            // Player Name
            for (int l = 0; l <= 2; l++)
            {
                if (l == 0) // Box tops
                {
                    Console.SetCursorPosition(left, top);
                    Console.Write("┌", color);

                    foreach (var chr in word)
                    {
                        Console.Write("─", color);
                    }

                    Console.Write("┐", color);
                }
                else if (l == 1) // Box contents
                {
                    Console.SetCursorPosition(left, top + 1);
                    Console.Write($"│{word}│", color);
                }
                else if (l == 2) // Box bottoms
                {
                    Console.SetCursorPosition(left, top + 2);

                    Console.Write("└", color);

                    foreach (var chr in word)
                    {
                        Console.Write("─", color);
                    }

                    Console.Write("┘", color);
                }
            }
        }

        public static void WriteAt(int left, int top, string s, Color color)
        {
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            Console.SetCursorPosition(left, top);
            Console.Write(s, color);
            Console.SetCursorPosition(currentLeft, currentTop);
        }

        public static void EraseSection(int width, int lines, int left, int top)
        {
            for (int i = top; i < top + lines; i++)
            {
                for (int j = left; j < left + width; j++)
                {
                    WriteAt(j, i, " ", Color.White);
                }
            }
        }

        public static void WriteWordWrapAt(int width, int left, int top, string paragraph, Color color)
        {
            string[] lines = paragraph
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 0; i < lines.Length; i++)
            {
                string process = lines[i];
                List<String> wrapped = new List<string>();

                while (process.Length > width)
                {
                    int wrapAt = process.LastIndexOf(' ', Math.Min(width - 1, process.Length));
                    if (wrapAt <= 0) break;

                    wrapped.Add(process.Substring(0, wrapAt));
                    process = process.Remove(0, wrapAt + 1);
                }

                foreach (string wrap in wrapped)
                {
                    WriteAt(left, top, wrap, color);
                    top++;
                }

                WriteAt(left, top, process, color);
            }
        }

        public static void DrawBorder(Color color, int originX, int originY, int width, int height, bool doubleBorder = false)
        {
            // i = Column
            for (int i = originX; i < width + originX; i++)
            {
                // j = Row
                for (int j = originY; j < height + originY; j++)
                {
                    if (j == originY)
                    {
                        if (i == originX)
                            WriteAt(i, j, "┌", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "┐", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + 1 && doubleBorder)
                    {
                        if (i == originX)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + 2)
                            WriteAt(i, j, "┌", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + width - 3)
                            WriteAt(i, j, "┐", color);
                        else if (i != originX + 1 && i != originX + width - 2)
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + height - 1)
                    {
                        if (i == originX)
                            WriteAt(i, j, "└", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "┘", color);
                        else
                            WriteAt(i, j, "─", color);
                    }
                    else if (j == originY + height - 2 && doubleBorder)
                    {
                        if (i == originX)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + 2)
                            WriteAt(i, j, "└", color);
                        else if (i == originX + width - 1)
                            WriteAt(i, j, "│", color);
                        else if (i == originX + width - 3)
                            WriteAt(i, j, "┘", color);
                        else if (i != originX + 1 && i != originX + width - 2)
                            WriteAt(i, j, "─", color);
                    }
                    else
                    {
                        if (i == originX || (i == originX + 2 && doubleBorder))
                            WriteAt(i, j, "│", color);
                        else if (i == originX + width - 1 || (i == originX + width - 3 && doubleBorder))
                            WriteAt(i, j, "│", color);
                    }
                }
            }
        }
    }
}
