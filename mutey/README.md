# Mutey
A software utility for controlling microphone mute with an input device or on-screen button.

## Features
- Mutes the default microphone on Windows (tested on Windows 10 only)
- Software button (popup) to both show current state and toggle mute/unmute state
- Support for serial based input devices (the example I built uses an Arduino Nano and a piano foot pedal)

## Project aims
Overall the aim is to provide a software interface for Windows that can listen on a COM port for physical button presses and toggle the mute on conferencing software like Microsoft Teams and Discord

- [ ] Automatically start as a Windows tray app on logon
- [x] Auto detect plugged in device
- [ ] ~~Auto detect conferencing software startup~~
- [x] Toggle mute without switching active application
- [x] Display current state of mute