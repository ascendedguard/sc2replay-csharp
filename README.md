sc2replay-csharp
================
#### C# library for extracting player and game information from .sc2replay files. ####

This is an easy-to-use library to quickly parse and extract information from the undocumented .sc2replay format. The project has a side goal of creating an in-depth wiki with replay file documentation.

**Requirements:** Visual C++ 2010 Runtime Redistributable as a requirement to MpqLib.dll

Current Abilities
================
* Parses basic player and matchup information.
* Parses entire chat log.
* Parses all game events.
* Can insert lines of text into the chat log for any player.
* Can clear the in-game chat for a "clean" replay.

You can find and contribute information in deconstructing the file format in the wiki: <https://github.com/ascendedguard/sc2replay-csharp/wiki>

### Events parser

The events parser parses all of the events that the replay contains.  This includes selection, hotkey, camera, and ability events.  More technical information about the events can be found at [replay.game.events](https://github.com/Mischanix/sc2replay-csharp/wiki/replay.game.events).

The behavior of this events parser is dependent on a database which provides version-specific information on ability and unit ids and version-independent information on abilities, units, and events.  More information about the format and generation of this database can be found at [Event Information Database](https://github.com/Mischanix/sc2replay-csharp/wiki/Event-Information-Database).  A default database is compiled with the DLL, but it can be overridden by placing the .dat files into the application path, as needed.

**Starcraft2.ReplayParser.DataCompiler** is provided to translate an SQLite database to a set of .dat files.  For editing the database, I recommend using [SQLite Manager](https://addons.mozilla.org/en-US/firefox/addon/sqlite-manager/) with a beta version of Firefox (for SQLite 3.7.11).  Using the data compiler requires the System.Data.SQLite library, which is available at [system.data.sqlite.org](http://system.data.sqlite.org/index.html/doc/trunk/www/downloads.wiki) -- look for the x86 .NET 4.0 installer, and during setup, add the assemblies to the Global Assembly Cache.
