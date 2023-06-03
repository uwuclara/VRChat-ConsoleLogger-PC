using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace VRChat_ConsoleLogger_PC;

internal static class HandleEvents
{
    private const string urlPattern = @"(https?://[^\s]+)";
    private static int oscPort = 9000;
    private static readonly Dictionary<string, string> _dictionary = new ();

    internal static void display(List<string> lines)
    {
        foreach (var line in lines)
        {
            if (line.Contains("Destination set: "))
            {
                var msg = line.Split(new[] { "Destination set: " }, StringSplitOptions.None)[1];
                    
                Logger.msg(ConsoleColor.Cyan, "Instance Link: " + msg, eventType.Travelig);
            }
            else if (line.Contains("OnPlayerJoined "))
            {
                var msg = line.Split(new[] { "OnPlayerJoined " }, StringSplitOptions.None)[1];

                Logger.msg(ConsoleColor.Yellow, msg, eventType.Joined);
            }
            else if (line.Contains("OnPlayerLeft "))
            {
                var msg = line.Split(new[] { "OnPlayerLeft " }, StringSplitOptions.None)[1];

                Logger.msg(ConsoleColor.Yellow, msg, eventType.Leave);
            }
            else if (line.Contains("Entering Room: "))
            {
                var msg = line.Split(new[] { "Entering Room: " }, StringSplitOptions.None)[1];

                Logger.msg(ConsoleColor.Cyan, "Joining World: " + msg, eventType.World);
            }
            else if (line.Contains("Microphone device changing to "))
            {
                var msg = line.Split(new[] { "Microphone device changing to " }, StringSplitOptions.None)[1];

                if (_dictionary.TryGetValue("lastMicChangeMsg", out var lastMicChangeMsg))
                {
                    if (lastMicChangeMsg != msg)
                    {
                        Logger.msg(ConsoleColor.Gray, "Microphone changed to: " + lastMicChangeMsg, eventType.Info);
                    }
                }
                else
                {
                    Logger.msg(ConsoleColor.Gray, "Microphone changed to: " + msg, eventType.Info);
                }
                
                _dictionary["lastMicChangeMsg"] = msg;
            }
            else if (line.Contains("VRChat Build: "))
            {
                Logger.msg(ConsoleColor.Gray, line.Trim(), eventType.Info);
            }
            else if (line.Contains("Using network server version: "))
            {
                var msg = line.Split(new[] { "Using network server version: " }, StringSplitOptions.None)[1];
                
                Logger.msg(ConsoleColor.Gray, "Network Ver.: " + msg, eventType.Info);
            }
            else if (line.Contains("User Authenticated: "))
            {
                var msg = line.Split(new[] { "User Authenticated: " }, StringSplitOptions.None)[1];
                
                Logger.msg(ConsoleColor.Gray, "Logged in as: " + msg, eventType.Info);
            }
            else if (line.Contains("Could not Start OSC: Address already in use"))
            {
                Logger.error("Found VRChat child not properly killed for OSC port to be used, will try auto-killing the child. Make sure to disable & enable OSC in ActionMenu!");
                    
                Wrapper.killOSCChid(oscPort);
            }
            else if (line.Contains("OSC:: "))
            {
                var msg = line.Split(new[] { "OSC:: " }, StringSplitOptions.None)[1];
                
                const string pattern = @"\b\d{4}\b";
                var regex = new Regex(pattern);
                var matches = regex.Matches(msg);
                
                if (matches.Count > 0)
                {
                    oscPort = int.Parse(matches[0].Value);
                }

                Logger.msg(ConsoleColor.Gray, "OSC: " + msg, eventType.Info);
            }
            else if (line.Contains("Attempting to load String from URL "))
            {
                var msg = line.Split(new[] { "Attempting to load String from URL " }, StringSplitOptions.None)[1];
                
                Logger.msg(ConsoleColor.Gray, "World is fetching string: " + msg, eventType.Info);
            }
            else if (line.Contains("[Video Playback] URL "))
            {
                var regex = new Regex(urlPattern);
                var matches = regex.Matches(line);

                if (matches.Count > 0)
                {
                    // var resolvedURL = matches[1].Groups[1].Value;
                    var url = matches[0].Groups[1].Value;
                    
                    if (_dictionary.TryGetValue("lastVideoPlayMsg", out var lastVideoPlayMsg))
                    {
                        if (lastVideoPlayMsg != url)
                        {
                            Logger.msg(ConsoleColor.Yellow, "Playing video: " + url[..^1], eventType.VideoPlayer);
                        }
                    }
                    else
                    {
                        Logger.msg(ConsoleColor.Yellow, "Playing video: " + url[..^1], eventType.VideoPlayer);
                    }
                
                    _dictionary["lastVideoPlayMsg"] = url;
                }
            }
            else if (line.Contains("[VRCX]"))
            {
                var msg1 = line.Split(new[] { "[VRCX] " }, StringSplitOptions.None)[1];
                var msg = msg1.Split(new[] { "," }, StringSplitOptions.None);

                string user, video;
                
                if (msg.Last().Contains("720p") || msg.Last().Contains("1080p") || msg.Last().Contains("480p"))
                {
                    if (string.IsNullOrEmpty(msg.ElementAt(msg.Length - 3)) ||
                        string.IsNullOrEmpty(msg.ElementAt(msg.Length - 2))) return;
                    
                    user = msg.ElementAt(msg.Length - 3);
                    video = msg.ElementAt(msg.Length - 2);
                }
                else
                {
                    if (string.IsNullOrEmpty(msg.Last()) ||
                        string.IsNullOrEmpty(msg.ElementAt(msg.Length - 2))) return;
                    
                    user = msg.ElementAt(msg.Length - 2);
                    video = msg.Last();
                }
                
                if (float.TryParse(user, out var lul4) && float.TryParse(video, out var lul5)) continue;
                
                if (_dictionary.TryGetValue("lastVRCXUserMsg", out var lastVRCXUserMsg) && _dictionary.TryGetValue("lastVRCXVideoMsg", out var lastVRCXVideoMsg))
                {
                    if (lastVRCXVideoMsg != video)
                    {
                        Logger.msg(ConsoleColor.Yellow, "VRCX Supported World: User (" + user +") put on video (" + video + ")", eventType.VideoPlayer);
                    }
                    else
                    {
                        if (!float.TryParse(user, out var lul))
                        {
                            if (lastVRCXUserMsg != user)
                            {
                                Logger.msg(ConsoleColor.Yellow, "VRCX Supported World: User (" + user +") put on video (" + video + ")", eventType.VideoPlayer);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    Logger.msg(ConsoleColor.Yellow, "VRCX Supported World: User (" + user +") put on video (" + video + ")", eventType.VideoPlayer);
                }
            
                _dictionary["lastVRCXUserMsg"] = user;
                _dictionary["lastVRCXVideoMsg"] = video;
            }
        }
    }

    public enum eventType
    {
        None = ConsoleColor.White,
        Joined = ConsoleColor.Green,
        Leave = ConsoleColor.Red,
        Travelig = ConsoleColor.Magenta,
        World = ConsoleColor.Magenta,
        APP = ConsoleColor.DarkMagenta,
        VideoPlayer = ConsoleColor.DarkCyan,
        Info = ConsoleColor.Yellow
    }
}