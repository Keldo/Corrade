///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2016 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;

namespace wasSharpNET.Console
{
    public static class ConsoleExtensions
    {
        public static void WriteLine(this object data, ConsoleColor foreground = ConsoleColor.White,
            ConsoleColor background = ConsoleColor.Black)
        {
            var cFG = System.Console.ForegroundColor;
            var cBG = System.Console.BackgroundColor;
            System.Console.ForegroundColor = foreground;
            System.Console.BackgroundColor = background;
            System.Console.WriteLine(data);
            System.Console.ForegroundColor = cFG;
            System.Console.BackgroundColor = cBG;
        }

        public static void Write(this object data, ConsoleColor foreground = ConsoleColor.White,
            ConsoleColor background = ConsoleColor.Black)
        {
            var cFG = System.Console.ForegroundColor;
            var cBG = System.Console.BackgroundColor;
            System.Console.ForegroundColor = foreground;
            System.Console.BackgroundColor = background;
            System.Console.Write(data);
            System.Console.ForegroundColor = cFG;
            System.Console.BackgroundColor = cBG;
        }
    }
}