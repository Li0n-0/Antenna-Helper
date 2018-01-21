# Antenna-Helper

Ever wonder what all the antennas you put on your craft, because they look so cool, actually do ? How far can you go with them ? How many science you'll be able to send back when you get there ?

It's easy ! Just multiply the power of your strongest antenna with the result of the division of the sum of all your antenna's power by the power of the strongest antenna raised by their average weighted combinability exponent. This will give you the antenna power of your spacecraft, now calculate the root square of this number multiplied by the DSN power, it will give you your maximum range. So simple.

**OR**

You could use Antenna Helper to do all that, and more, for you.



## Ok, but what does it do ?

It will show you the antenna capability of your active vessel in flight, in the editor and of **all** vessels in the tracking station.


## In flight ?

Only in the **Map View** (for now). Click the Antenna Helper icon on the app launcher : ![Antenna Helper icon](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/icon_off.png?raw=true)
And you should see something close to that :

![map view active connect](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/map_view_active.png?raw=true)


Those color circles represent the maximum range between your vessel and its relay/DSN, it also indicate how much signal strength you get. While your ship is in the green circle its signal strength will be clamped between 100 and 75%, in the yellow circle between 75 and 50%, in the orange circle between 50 and 25%, in the red circle between 25 and 0%. And if you're outside the red circle you don't have any connection.


Apart from the color circles, you'll see a small window next to Antenna Helper icon on the app launcher : 

![map view window](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/map_view_window.png?raw=true)


The four button will show you your range, and signal strength, from different sources (relay or DSN).

* **ACTIVE** (by default) show your range for the active connection, which can be from a relay or DSN.

![map view active connect](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/map_view_active.png?raw=true)


* **DSN** show the maximum range between your vessel and the DSN

![map view dsn connect](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/map_view_dsn.png?raw=true)


* **RELAY** show the maximum range between your vessel and all the in-flight relay

![map view relay connect](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/map_view_relay.png?raw=true)


* **DSN and RELAY** combine the two above, showing you **all** the possible connection, with their range

![map view relay and dsn connect](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/map_view_dsn_and_relay.png?raw=true)



## You mentioned the editor ?

Yep. That's nice to see "on live" where you can go with your space probe but it's even better to know before launch how far you can travel.

In the VAB (or SPH) click the Antenna Helper icon on the app launcher : ![Antenna Helper icon](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/icon_off.png?raw=true)
It brings this window :

![editor main window direct](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/editor_main_window_direct.png?raw=true)


Lots of infos here, let's go through them from top to bottom :
* **Selected type :** which antennas are taking into account to compute the vessel antenna power. **Direct** (by default) will use all antennas, **Relay** will use only the antenna with relay capability.
* **Current target :** the range of your ship's antenna is determined by the antenna power of its target, which can be the DSN or a relay. By default the selected target is the DSN. You can change the target by clicking on **Pick A Target**. (more about it below)
* **Status :** a quick explanation about which antenna on your craft will actually be used in flight.
* **Power :** the antenna power of your vessel.
* **Max Range :** the maximum distance between your vessel and the target after which you'll lose the connection. Depend on the power of your vessel **and** the power of the target.
* **Max Distance At 100% :** the distance between your vessel and the target after which your signal strength will start to decay.
* **Color bar :** it works with the **Max Distance At 100%** and the **Max Range** distance. The number aligned with the black bars separating the colors indicate the distance at which the signal decay. Example from the pic above : between 2 050 205 945m and 51 662 800 363m, your signal strength will vary from 100% to 75%.


## So the target is important ?

Of course, one antenna on its own don't do anything. It must be connected to another antenna, can't compute maximum range or signal strength with only one antenna.
Two type of target :
* **the DSN (Deep Space Network) :** it's the (very) big antenna on Kerbin. It has three levels with different power, you upgrade from one to another by upgrading the Tracking Station.
* **relay(s) :** those are vessels you build yourself, two condition need to be meet : having at least one antenna with relay capability and setting the vessel type to "Relay".

You can simulate all those connection directly from the editor : Click the **Pick A Target** button : 

![editor target dsn window](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/editor_target_dsn_window.png?raw=true)


From here you can select a different DSN level, your current level is in **bold** and is selected by default. For simulating against in-flight relay hit the **In-Flight Ships** button :

![editor target flight window](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/editor_target_flight_window.png?raw=true)


In this window you should see one button per flying vessel with its name and its antenna **relay** power, important distinction, when your connection is going through a relay only the relay antenna of this relay will be used. Sound obvious but... So if your building a relay make sure your selected type, in the main window, is set to **RELAY**.

Speaking of building relays, you may want to simulate the antenna range of a relay before you launch it. To do so you need to add your ship/relay to the **Antenna Helper Editor Ship List**. Just open your vessel in the editor and click on **Add Ship to the Target List** in the main window. After which you can click on **Editor Ships** in the **Pick A Target** window : 

![editor target editor window](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/editor_target_editor_window.png?raw=true)


It works the same way as the in-flight relay list, the number between parenthesis is still the antenna **relay** power.



## But what all those numbers really mean ?

They are, mostly, just distance. At a solar system scale. So big distance, like space-travel distance (we're playing a space program game, are we not ?).
Anyway, to help figuring out what does numbers represent you'll find this window, by clicking on **Signal Strength / Distance** in the main window :

![editor signal per distance window](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/editor_signal_distance_window.png?raw=true)


In it you'll see what signal strength to expect for a distance equal to the minimum and maximum distance between the home body (Kerbin), its moon(s) and all the others planet on the solar system. Keep in mind that those distance are approximate, specially for celestial body with an highly inclined orbit. 
You can check the distance used by hovering your mouse on the celestial body name.
In the same window you can check the signal strength to expect at any given distance, write it in the input box at the bottom of the window and click the **Math !** button.


Still don't get a clear representation of what those space-travel distance mean ?
Save your ship in the **Antenna Helper Editor Ship List**, click the **Add Ship to the Target List** button in the main window. Quit the editor and open up the **Tracking Station**.



## Tracking Station ? Yummy !

Yes it's good. Fire Antenna Helper : ![Antenna Helper icon](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/icon_off.png?raw=true)

From there if you select a vessel in the Tracking Station list you'll see its range circles just like in flight. You can check the range for the active connection, the DSN connection and for all relay in flight by selecting it in the GUI.
To check the range of a vessel saved in the Editor click the **Editor Ship List** button.

![tracking station editor ship list window](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/tracking_station_editor_ship_list_window.png?raw=true)


Now you can see the range and the signal strength of your future vessel for the different connection type.
The number between parenthesis is the **total** antenna power of your vessel, as opposed to the Ship List in the Editor that show the **relay** power.

![tracking station editor ship](https://github.com/Li0n-0/Antenna-Helper/blob/master/pics/tracking_station_editor_ship.png?raw=true)





## About the mod

This is still a work in progress, you can use it safely, it won't break any thing in your game but the value may be off. If you find a bug or have any suggestion please post [on this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/156122-wip122-to-131-antenna-helper-in-game-antennas-range-calculation-v07-7-oct-2017/) on the official KSP forum or create an issue [on GitHub](https://github.com/Li0n-0/Antenna-Helper).

### Know issues :

* ~~the circle of the map view jitter at high time-wrap. Almost fixed.~~
* the orientation of the map view circle, relative to the camera, is, most of the time, sub-optimal. Fixed when orbiting the DSN's planet
* transparency of the circle are not good, specially when they overlap.
* ~~in-flight, antennas of the active ship are all considered extended.~~
* ~~in-flight math are done only once on loading. It need to be re-done when the ship stage, dock, etc...~~
* ~~DSN and range modifier may not be corectly set when loading a new game (after an "exit to main menu")~~
* range modifier are off when using RSS, see [this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/156122-wip131-antenna-helper-in-game-antennas-range-calculation-v09-8-dec-2017/&do=findComment&comment=3240861) should be fixed by v0.13, waiting confirmation
* circles in the Tracking Station disappear when zoomed far away
* Tracking Station window should be clamped to the button

### Future plans :

* ~~showing the range circle in the tracking station.~~ **Done**
* ~~+ show range circle for ship not already launched.~~ **Done**
* Window with NUMBERS in flight.
* in the editor, a window showing a list of antenna with their caracteristics.
* ~~in the editor, add all the in-flight relay to the list of target.~~ **Done**
* in the editor, add relay antennas (part) to the list of target.
* re-work the GUI, possibly with the new GUI system instead of on OnGUI.
* ~~map view window should be clamped to the toolbar button.~~ **Done**


## Credits

The idea for an in-game calculator is from [this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/153155-mod-idea-in-game-antenna-strength-calculater/) by [Tyko](https://forum.kerbalspaceprogram.com/index.php?/profile/164179-tyko/).

Thanks to [Poodmund](https://forum.kerbalspaceprogram.com/index.php?/profile/128643-poodmund/) for his [google docs's calculator](https://docs.google.com/spreadsheets/d/1qIgFB8OXnlgpPCGsxv7JYUYQq5O671IcZXpumVaStek/edit?usp=sharing), and for the help he provide to this mod.

Thanks to [Skalou](https://forum.kerbalspaceprogram.com/index.php?/profile/133496-skalou/) for his help with the math :wink:

Antenna Helper icon/logo made by myself with assets from [FlatIcon](https://www.flaticon.com) by [Freepik](https://www.flaticon.com/authors/freepik).

## [Thread on offical KSP forum](https://forum.kerbalspaceprogram.com/index.php?/topic/156122-wip131-antenna-helper-in-game-antennas-range-calculation-v08-17-nov-2017/)
