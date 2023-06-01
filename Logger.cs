using System;

namespace VRChat_ConsoleLogger_PC;

internal static class Logger
{
    internal static void msg(ConsoleColor color, object msg, HandleEvents.eventType type)
    {
        var defaultColor = Console.ForegroundColor;
        Console.Write("[" + DateTime.Now.ToLocalTime().ToString("MM/dd/y hh:mm:ss tt") + "]");
        Console.ForegroundColor = (ConsoleColor) type;
        Console.Write("[" + Enum.GetName(typeof(HandleEvents.eventType), type) + "] ");
        Console.ForegroundColor = color;
        Console.Write(msg + Environment.NewLine);
        Console.ForegroundColor = defaultColor;
    }

    internal static void error(object msg)
    {
        var defaultColor = Console.ForegroundColor;
        Console.Write("[" + DateTime.Now.ToLocalTime().ToString("MM/dd/y hh:mm:ss tt") + "]");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("[ERROR] ");
        Console.Write(msg + Environment.NewLine);
        Console.ForegroundColor = defaultColor;
    }
}