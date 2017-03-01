TomiSoft MP3 Player
===================

[![Build status](https://ci.appveyor.com/api/projects/status/9yv8gg2qf1c80g2d?svg=true)](https://ci.appveyor.com/project/std66/tomisoft-mp3-player)

An advanced audio playback application built on the BASS library.
  - Supports 36 audio file formats: mp3, ogg, wav, mp2, mp1, aiff, m2a, mpa, m1a, mpg, mpeg, aif, mp3pro, bwf, mus, wma, wmv, aac, adts, mp4, m4a, m4b, cda, flac, midi, mid, rmi, kar, wma, wmv, aac, adts, mp4, m4a, m4b, ac3
  - Karaoke-like lyrics displaying (supports LRC and XML-format (with multiple translation support) lyrics files)
  - VU meter
  - API provided over TCP connection on loopback interface
  
Download binary:
----------------
http://tomisoft.site90.net/TomiSoft-MP3-Player/TomiSoft-MP3-Player.zip

Screenshots:
------------
![Screenshot 1](https://github.com/std66/TomiSoft-MP3-Player/raw/master/Screenshots/Screen1.png "Main screen")
![Screenshot 2](https://github.com/std66/TomiSoft-MP3-Player/raw/master/Screenshots/Screen2.png "Playlist")
  
Notes:
------
  - Mid, midi, rmi and kar files can be opened, but requires a SoundFont. However, this function is not implemented yet, so you will hear no sound.
  - Lyrics translation can only be changed via TCP API connection

System requirements:
--------------------
  - Microsoft Windows 8.1 or newer operating system
  - Microsoft .NET Framework 4.6.1
  
Author:
-------
Sinku Tamás (sinkutamas@gmail.com)
