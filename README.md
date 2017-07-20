TomiSoft MP3 Player
===================

[![Build status](https://ci.appveyor.com/api/projects/status/9yv8gg2qf1c80g2d?svg=true)](https://ci.appveyor.com/project/std66/tomisoft-mp3-player)

An advanced audio playback application built on the BASS library.
  - Supports 36 audio file formats: mp3, ogg, wav, mp2, mp1, aiff, m2a, mpa, m1a, mpg, mpeg, aif, mp3pro, bwf, mus, wma, wmv, aac, adts, mp4, m4a, m4b, cda, flac, midi, mid, rmi, kar, wma, wmv, aac, adts, mp4, m4a, m4b, ac3
  - Karaoke-like lyrics displaying (supports LRC and XML-format (with multiple translation support) lyrics files)
  - Downloading and playing music from YouTube (see "YouTube support")
  - Saving the currently opened music (even from audio cd)
  - VU meter
  - API provided over TCP connection on loopback interface

Download:
---------
http://tomisoft.byethost7.com/TomiSoft-MP3-Player/TomiSoft-MP3-Player.zip

Screenshots:
------------
![Screenshot 1](https://github.com/std66/TomiSoft-MP3-Player/raw/master/Screenshots/Screen1.png "Main screen")
![Screenshot 2](https://github.com/std66/TomiSoft-MP3-Player/raw/master/Screenshots/Screen2.png "Playlist")
  
Notes:
------
  - Mid, midi, rmi and kar files can be opened, but requires a SoundFont. However, this function is not implemented yet, so you will hear no sound.

YouTube support:
----------------
YouTube support requires ffmpeg.exe, ffprobe.exe (LGPL 2.1, https://ffmpeg.org/) and youtube-dl.exe (The Unlicense, https://rg3.github.io/youtube-dl/). These are not the part of the package. At the first YouTube download the user will be prompted about downloading and installing these dependencies. The installation is automatic if the user agreed and may require administrative privileges.

Other third party components:
-----------------------------
  - The core of this application is built on the BASS library developed by Un4Seen: http://un4seen.com/ http://bass.radio42.com/
  - LAME encoder is used for ripping music from Audio CDs: http://lame.sourceforge.net
  - Font Awesome provides fancy icons: http://fontawesome.io/ https://github.com/charri/Font-Awesome-WPF
  - TagLib# is used to update ID3 tags: https://github.com/mono/taglib-sharp

System requirements:
--------------------
  - Microsoft Windows 8.1 or newer operating system
  - Microsoft .NET Framework 4.6.1
  
Author:
-------
Sinku Tamás (sinkutamas@gmail.com)
