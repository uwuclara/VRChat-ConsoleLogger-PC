# VRChat-ConsoleLogger-PC

VRChat ConsoleLogger for PC without bypass of EAC. 
Fully operates on logs therefore not bannable. 
I wanted to make this for a while, but got lazy.

## How To Use
Run the EXE file. I have no idea if these parms are needed, I always have it. Steam Lib -> VRChat -> Manage -> Properties -> General -> Launch Options (--enable-sdk-log-levels --enable-udon-debug-logging --log-debug-levels=API).
You can run this EXE file anytime (during the game or when VRChat is not opened).
Suports even when VRChat exits and start again. 

## What Events it supports?
* Player Join/Leave
* World change and it's instance link
* URL of strings the world is trying to download
* Microphone change
* VRChat Build (on vrc startup)
* Logged in as (on vrc startup)
* What network version (on vrc startup)
* OSC what ports is listening on and auto fix for VRChat failing to release it's port (it auto kills the child when detected, must disable/enable OSC in Action Menu)
* VideoPlayer URLs
* VRCX video details
