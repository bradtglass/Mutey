# HueMuteIndicator

*This project is a work in progress*

Joins up the Philips Hue and Windows Audio APIs to automatically apply lighting settings when the system microphone is in use. The application can be used to change a lights colour when you join a conference call in an application like Zoom or Microsoft Teams etc. 

## Credits
The article that inspired me to create this can be found [here](https://jussiroine.com/2020/06/building-a-custom-presence-light-solution-using-philips-hue-lights-and-c/), the main thing I added was the ability to configure the state you want to apply and caching of the last state so it can be reset. This article was a really great inspiration, thanks Jussi!

I use two great open source libraries in this project too:
- [Q42.HueApi](https://github.com/michielpost/Q42.HueApi)
- [NAudio](https://github.com/naudio/NAudio)

They aren't the only two but they are by far the most used here.
