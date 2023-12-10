# This mod fixes an issue with the boombox that happens if the boombox is already in the lobby that causes different songs to play between the client and host.

## What is this bug, exactly?

The way the boombox is usually supposed to behave is that whenever you buy it, every song that the host hears can also be heard by every client, which means that the boomboxes are synced between them. However, if you buy a boombox, leave the lobby, initiate a game where the boombox IS STILL IN THE SHIP, then the bug happens. Suddenly, as soon as you land your ship somewhere, the host hears a different song from the clients, which means, that somehow they got desynced, and will remain so for the rest of the game, until you buy a new boombox which will be synced again.

## What causes this bug?
It has to do with how the game handles the randomization of the boombox songs. Whenever you load into a game, the Object BoomboxItem gets created through this Start() method:

![StartMethod](https://github.com/milleroski/BoomboxSyncFix/assets/58141168/a3cf63bd-6e0c-4d5b-b052-46b1c22da762)

Keep your attention on the musicRandomizer variable, which is, as the name suggests, a variable which uses a seeded System.Random() to actually pick the song randomly whenever you start playing the music, which happens in the StartMusic() Method:

![StartMusicMethod](https://github.com/milleroski/BoomboxSyncFix/assets/58141168/4e6b8893-48f5-4eba-b233-a2e7d3bb539c)

Specifically, take note of the fact that the musicRandomizer gets only assigned ONCE, and then it stays like that for the rest of the game lobby. Now, here's the root cause of this: The randomMapSeed is not the same between the client and the host at the moment the BoomboxItem Start() method gets run.
Take note of the fact that the musicRandomizer gets only assigned ONCE, and then it stays like that for the rest of the game lobby. Now, here's the root cause of this: The randomMapSeed is not the same between the client and the host at the moment the BoomboxItem Start() method gets run.

### Why isn't the randomMapSeed the same between the client and the host?
It has to do with the way Lethal Company handles a client connecting to the game. Basically, from the clients side, it loads the boombox object and initializes the playersManager class at the same time. This is fine, however, the variables inside of playersManager, like the randomMapSeed do not get synced immediately with the server, it actually takes a bit of time (like half a second or so) to actually get the information from the server. In this small time gap the following happens -- the musicRandomizer for the client gets initialized with the following statement:
```c#
this.musicRandomizer = new Random(0 - 10);
```
The randomMapSeed is 0 because that's the default value before it has been synced up with the server. For the host it might look something like this instead:
```c#
this.musicRandomizer = new Random(54396478 - 10);
```
This means that randomMapSeed gets synced AFTER this initialization happens, ultimately resulting in two different seeds for the Random function for the client and the host, meaning that they're not synced up, causing two different songs to play.

## How did you fix this?
Well, I didn't really want to mess with the Netcode of Lethal Company, since that is what actually causes the bug. Specifically, the way it should work is that the server syncs the variables FIRST, and only then allows the player to actually initate the objects like the Boombox and others. I settled for another approach instead -- I just patch the StartMusic() function with a Harmony Prefix, and all I really do is just reinitialize the musicRandomizer variable after the seed gets actually synced properly (has a value that isn't 0), so now the client and the host both have the same musicRandomizer seed, meaning that they're synced up again.
