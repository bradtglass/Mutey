# Mutey
A software utility for working with the microphone(s) mute on Windows.

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

## Additional features
### Hue Indicator

*This is a work in progress*

Join up the Philips Hue API to automatically apply lighting settings when the system microphone is in use. The application can be used to change a lights colour when you join a conference call in an application like Zoom or Microsoft Teams etc. 

#### Credits
The article that inspired me to create this can be found [here](https://jussiroine.com/2020/06/building-a-custom-presence-light-solution-using-philips-hue-lights-and-c/), the main thing I added was the ability to configure the state you want to apply and caching of the last state so it can be reset. This article was a really great inspiration, thanks Jussi!

I use two great open source libraries in this project too:
- [Q42.HueApi](https://github.com/michielpost/Q42.HueApi)
- [NAudio](https://github.com/naudio/NAudio)

They aren't the only two but they are by far the most used here.
