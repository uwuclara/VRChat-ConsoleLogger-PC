using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace VRChat_ConsoleLogger_PC;

internal static class Wrapper
{
    internal static void killOSCChid(int oscPort = 9000)
    {
        var process1 = executeCMDCommand("netstat -aon | findstr " + oscPort);
        var output1 = process1.StandardOutput.ReadToEnd();
        process1.WaitForExit();

        var childPID = getLastNumber(output1);

        if (!string.IsNullOrEmpty(childPID))
        {
            Logger.msg(ConsoleColor.Gray, "Found Child PID using OSC port: " + childPID, HandleEvents.eventType.Info);

            var process2 = executeCMDCommand("wmic process get processid,parentprocessid | findstr /i " + childPID);
            var output2 = process2.StandardOutput.ReadToEnd();
            process2.WaitForExit();

            var parentPID = getLastNumber(output2);

            if (!string.IsNullOrEmpty(parentPID))
            {
                Logger.msg(ConsoleColor.Gray, "Found Parent PID using OSC post: " + parentPID, HandleEvents.eventType.Info);
                
                var process3 = executeCMDCommand("taskkill /f /pid " + parentPID);
                process3.WaitForExit();

                Logger.msg(ConsoleColor.Gray, "Process Killed, disable & enable OSC in Action Menu!", HandleEvents.eventType.Info);
            }
            else
            {
                Logger.error("Couldn't find parent PID that is using OSC Port.");
            }
        }
        else
        {
            Logger.error("Couldn't find child PID that is using OSC Port.");
        }
    }

    internal static Process executeCMDCommand(string command)
    {
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/C " + command;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        
        return process;
    }

    internal static string? getLastNumber(string input)
    {
        const string pattern = @"\b\d+\b";
        var matches = Regex.Matches(input, pattern);
        
        if (matches.Count > 0)
            return matches[^1].Value;
        
        return null;
    }
}