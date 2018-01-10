# Antenna-Helper

Ever wonder what all the antennas you put on your craft, because they look so cool, actually do ? How far can you go with them ? How many science you'll be able to send back when you get there ?

It's easy ! Just multiply the power of your strongest antenna with the result of the division of the sum of all your antenna's power by the power of the strongest antenna raised by their average weighted combinability exponent. This will give you the antenna power of your spacecraft, now calculate the root square of this number multiplied by the DSN power, it will give you your maximum range. So simple.

**OR**

You could use Antenna Helper to do all that, and more, for you.



## How it works ?

For the example we'll use this craft :

![example craft in editor](https://i.imgur.com/mpTWdHv.png)

There is three antennas on it : 
* the MK1 Command Pod has an INTERNAL antenna built-in, with a power of 5k
* a Communotron DSM-M1, a DIRECT antenna with a power of 2G
* an HG-5 High Gain Antenna, a RELAY antenna with a power of 5M


### In the editor : 

Click on the Antenna Helper icon on the app launcher : ![Antenna Helper icon](https://i.imgur.com/Ii6G55t.png)

It will bring this window : 

![Antenna Helper main editor window](https://i.imgur.com/Rk2JQu4.png)

1. The type of antenna you want info about. Direct, by default, will take into account all the antennas, DIRECT, RELAY and INTERNAL. Select Relay if you plan to build a relay, hmm, as only the relay antenna will be used by connection going through.
2. Your target and its power, your current DSN level is automatically selected. Click "Pick a Target" will bring this window :
![Antenna Helper select a target window](https://i.imgur.com/APtsurH.png)
With the available DSN level and their power, the one in bold is your current DSN level.
3. The status of the antennas on the craft, showing how many antennas are combined.
4. The total power of the vessel.
5. The maximum range, a.K.a the distance from the target after which you won't get any signal.
6. The distance from the target where your signal will start to decay.
7. The value in front of each black bars are the distance from the target at which the signal strength will "change color", 75%, 50%, 25%. The value at the extreme left is 0m from the target, the value at the extreme right is the "Max Range".
8. Open this window :

![Antenna Helper signal strength per distance window](https://i.imgur.com/XgaNVCd.png)
        1. Show the signal strength at the distance between the target (the home body) and its moons and the planet of the solar system. Hovering over the planet/moon name will show the distance used for the calcul. Those distance are approximate, specially for planet with high inclination.
        2. Enter any distance in the input box on the left, click "Math !" on the right, the signal strength to expect will show on the middle.


### In flight :

Go to the Map View, click the Antenna Helper icon on the app launcher : ![Antenna Helper icon](https://i.imgur.com/Ii6G55t.png)

(Zoom out)

![Antenna Helper map view dsn connection](https://i.imgur.com/N1uJ103.png)

The red circle is the maximum range, fly beyond it and you wont have any signal.
In the orange one the signal strength will vary from 50% to 25%.
In the yellow circle the signal strength will vary from 75% to 50%.
In the green circle the signal strength will vary from 100% to 75%.
Those circle match the value on the color bar of the editor window (item 7. above).

After clicking on the Antenna Helper icon you'll see a window right next to it : 

![Antenna Helper map view window](https://i.imgur.com/AQMotEv.png)

1. The current selection.
2. Four buttons :
    * Active Connection : will draw one set of range circle around the current target of the connection, DSN or relay.
	* DSN : will draw one set of circle around the DSN.
	* Relay : will draw one set of circle around each relay with an active connection.
	* DSN + Relay : the two above combined.

![Antenna Helper map view dsn and relay connection](https://i.imgur.com/NPIFmOD.png)




## About the mod

This is still a work in progress, you can use it safely, it won't break any thing in your game but the value may be off. If you find a bug or have any suggestion please post [on this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/156122-wip122-to-131-antenna-helper-in-game-antennas-range-calculation-v07-7-oct-2017/) on the official KSP forum or create an issue [on GitHub](https://github.com/Li0n-0/Antenna-Helper).

### Know issues :

* the circle of the map view jitter at high time-wrap. Almost fixed.
* the orientation of the map view circle, relative to the camera, is, most of the time, sub-optimal.
* transparency of the circle are not good, specially when they overlap.
* in-flight, antennas of the active ship are all considered extended.
* in-flight math are done only once on loading. It need to be re-done when the ship stage, dock, etc...
* DSN and range modifier may not be corectly set when loading a new game (after an "exit to main menu")

### Future plans :

* showing the range circle in the tracking station.
* + show range circle for ship not already launched.
* Window with NUMBERS in flight.
* in the editor, a window showing a list of antenna with their caracteristics.
* in the editor, add all the in-flight relay to the list of target.
* in the editor, add relay antennas (part) to the list of target.
* re-work the GUI, possibly with the new GUI system instead of on OnGUI.
* map view window should be clamped to the toolbar button.


## Credits

The idea for an in-game calculator is from [this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/153155-mod-idea-in-game-antenna-strength-calculater/) by [Tyko](https://forum.kerbalspaceprogram.com/index.php?/profile/164179-tyko/).

Thanks to [Poodmund](https://forum.kerbalspaceprogram.com/index.php?/profile/128643-poodmund/) for his [google docs's calculator](https://docs.google.com/spreadsheets/d/1qIgFB8OXnlgpPCGsxv7JYUYQq5O671IcZXpumVaStek/edit?usp=sharing), and for the help he provide to this mod.

Thanks to [Skalou](https://forum.kerbalspaceprogram.com/index.php?/profile/133496-skalou/) for his help with the math :wink:

Antenna Helper icon/logo made by myself with assets from [FlatIcon](https://www.flaticon.com) by [Freepik](https://www.flaticon.com/authors/freepik).

## [Thread on offical KSP forum](https://forum.kerbalspaceprogram.com/index.php?/topic/156122-wip131-antenna-helper-in-game-antennas-range-calculation-v08-17-nov-2017/)
