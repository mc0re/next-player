# NexT Player

This is a theatrical (mainly) audio player. The goal is to allow setting up the performance beforehand, such that the main command performed during the performance is "Next".

Points of interest:

- Waveform visualization
- Non-destructive clipping
- Fully adjustible in-between delays
- Parallel and cross-fade playback
- 3D sound
- Volume automation
- Voice control
- Playlist printing
- PowerPoint integration
- Show text windows on the screen, the text can be moving (simple karaoke)

User manual is in `NextPlayer\Resources\LocalHelp.rtf` file.

## History

The project started about 2013.
The binaries for 2.1.2 are here: http://nextplayer.nikitins.dk/

## Release

1. Change the version in `SetupNextPlayer\SetupNextPlayer.vdproj`, tag `ProductVersion`
2. Run this command from _Developer Powershell_ to build:

```
devenv.com .\NextPlayer.sln /Build Release
```

3. Copy `..\Setup\*.msi` to the cloud using e.g. _Filezilla_.
