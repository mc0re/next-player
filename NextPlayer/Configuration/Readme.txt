The configurations are divided by two orthogonal dimensions.

1-st dimension
- Configuration saved locally on a computer
	Only used on this computer; e.g. window position, voice control config
- Configuration saved in the playlist
	Distributed via the playlist; e.g. audio and text physical channels, room definition

2-nd dimension
- General configuration
	There is only one instance of it
- Environment configuration
	One instance per environment

The classes responsible are:
	Computer, general:		AppConfiguration
	Computer, environment:	AppEnvironmentConfiguration
	Playlist, general:		PlayerActionCollection (part of the playlist is the configuration properties)
	Playlist, environment:	PlaylistEnvironmentConfiguration

The current environment is changed by setting
AppConfiguration.EnvironmentName property,
which in turn sets PlayerActionCollection.EnvironmentName property.
