# GoStreamAudio
music player in C# based on NAudio

The solution is split in 2 projects: 
- GoStreamAudioGUI : contains the graphical UI based on Windows Forms
- GoStreamAudioLib : the library for player functionalities which uses NAudio libraries

# What it can do
The player currently supports these file types: MP3, M4A, WMA, Wave, Vorbis OGG, FLAC.
It can play a single file or multiple files from a playlist.
In addition, it is possible to load or save a playlist, or remove all the entries from current playlist. 

Supported playlist formats: M3U

# Next things to do
- Playlist continuous repeat mode
- Play audio CDs
- ...

# Included external libraries
The application uses the following external libraries (some of them were adapted to .NET version 3.5):
- NAudio (https://github.com/naudio/NAudio) 
- NAudio.Flac (https://github.com/nharren/NAudio.Flac)
- NAudio.Vorbis (https://github.com/naudio/Vorbis)
- NVorbis (http://nvorbis.codeplex.com)
- Luminescence.Xiph (http://xiph.codeplex.com)
- taglib-sharp (https://github.com/mono/taglib-sharp/)
- Notification-Popup-Window (https://github.com/Tulpep/Notification-Popup-Window)

# Build and Test
To compile the following is required:
- .NET 3.5 and Visual Studio 2013

# Contribute
Feel free to contribute if you like. 

Have a nice day!
