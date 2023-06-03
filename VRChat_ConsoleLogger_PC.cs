using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace VRChat_ConsoleLogger_PC;

public static class VRChat_ConsoleLogger_PC
{
    public const string Name = "VRChatConsoleLoggerPC";
    public const string Version = "1.0.3";
    private static long readOffset;
    private static bool needClean;

    public static void Main()
    {
        Console.Title = "VRChatConsoleLoggerPC by uwuclara v" + Version;
        Console.OutputEncoding = Encoding.Unicode;
        
        Logger.msg(ConsoleColor.DarkGreen, "VRChatConsoleLoggerPC by uwuclara v" + Version, HandleEvents.eventType.APP);
        Logger.msg(ConsoleColor.DarkGreen, "Waiting for VRChat process", HandleEvents.eventType.APP);
        
        startLogScan();
    }

    private static void startLogScan()
    {
        while (true)
        {
            while (Process.GetProcessesByName("VRChat").Length == 0) 
                Thread.Sleep(1);

            var processes = Process.GetProcessesByName("VRChat");
            if (processes is { Length: > 0 })
            {
                var VRChatProcess = processes[0];

                if (VRChatProcess.HasExited) 
                    continue;

                if (needClean)
                {
                    Console.Clear();
                    needClean = false;
                    Logger.msg(ConsoleColor.DarkGreen, "VRChatConsoleLoggerPC by uwuclara v" + Version, HandleEvents.eventType.APP);
                }

                Logger.msg(ConsoleColor.DarkGreen, "VRChat process [" + VRChatProcess.Id + "]", HandleEvents.eventType.APP);

                var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"Low\VRChat\VRChat";
                var directory = new DirectoryInfo(path);

                if (directory.Exists)
                {
                    Logger.msg(ConsoleColor.DarkGreen, "Waiting for VRChat LogFile", HandleEvents.eventType.APP);

                    FileInfo? selectedLogInfo = null;
                    while (selectedLogInfo == null)
                    {
                        foreach (var fileInfo in directory.GetFiles("output_log_*.txt", SearchOption.TopDirectoryOnly))
                        {
                            if (VRChatProcess.StartTime.CompareTo(fileInfo.LastWriteTime) <= 0) selectedLogInfo = fileInfo;
                        }

                        Thread.Sleep(1);
                    }

                    Logger.msg(ConsoleColor.DarkGreen, "VRChat LogFile found  (" + selectedLogInfo.Name + ")", HandleEvents.eventType.APP);

                    Logger.msg(ConsoleColor.DarkGreen, "VRChat Logging starting", HandleEvents.eventType.APP);
                    Console.WriteLine();

                    readLines(selectedLogInfo.FullName, true);

                    while (!VRChatProcess.HasExited)
                    {
                        var lines = readLines(selectedLogInfo.FullName);
                        HandleEvents.display(lines);
                        Thread.Sleep(1);
                    }

                    Logger.msg(ConsoleColor.DarkGreen, "VRChat Exited", HandleEvents.eventType.APP);
                    readOffset = 0;
                    needClean = true;

                    Logger.msg(ConsoleColor.DarkGreen, "Waiting for VRChat process", HandleEvents.eventType.APP);
                }
                else
                {
                    Logger.error("VRChat Folder not found!");
                    
                    while (true)
                        Thread.Sleep(1);
                }
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    // from umba and edited
    private static List<string> readLines(string filePath, bool toEnd = false)
    {
        List<string> lines = new();

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);

            if (!toEnd)
            {
                reader.BaseStream.Seek(readOffset, SeekOrigin.Begin);
                
                while (reader.ReadLine() is { } line)
                {
                    lines.Add(line);
                }
            }
            else
            {
                reader.BaseStream.Seek(0, SeekOrigin.End);
            }

            readOffset = reader.BaseStream.Position;
        }
        catch (IOException ex)
        {
            Logger.error(ex);
        }

        return lines;
    }
}