# Augmented Tyria 0.3.1-beta

# PLEASE check in this file before asking questions

If you're already quite familiar with AugTyr 0.2, you only need to read sections labeled as **0.3** (use Ctrl + F).

## Running AugTyr

Download and extract the archive, then run the AugTyr.exe file alongside Guild Wars 2. The game must be in **windowed fullscreen** mode for AugTyr to work.

## Node types

* *Reach node (pink)*: You're just meant to get to it.

* **0.3** *Teleport node (cyan)*: Get to it then teleport to the next node. It should have some text attached to specify how to teleport to the next node unless it's obvious. Some possibilities are:

  * Using a waypoint. These teleport nodes will have the relevant waypoint code attached to them, which can be easily copied.
 
  * Entering or leaving an instance. These are both considered "teleports" as there is no limit to the distance immediately traveled in those situations.
 
  * Map-specific teleports, e.g. the Divinity's Reach tunnels, Black Citadel elevators, [Mad Mardine's cattlepult](https://wiki.guildwars2.com/wiki/Mad_Mardine)...

* *Heart node (yellow)*: Get to it then get working on a heart.

* *Heart wall node (orange)*: Don't continue past it until you have the specified % on the nearby heart.

## **0.3** Report console

AugTyr now comes with a report console where helpful messages are be displayed.

Some messages fade out on their own, others stay. The console can be cleared or hidden. Default keybindings for these actions are outlined below.

There are three types of messages:

* *Info (black)*: Reports on what AugTyr is generally doing.

* *Warning (yellow)*: Reports on wrong inputs that should be noted but can be safely ignored.

* *Error (red)*: Reports on problems that the current operation cannot continue because of.

The console maintains the smallest size necessary to display all of its messages. When there are no messages, the console will be tiny but still visible (unless specifically hidden).

## **0.3** Unofficial routes

Routes take me a long time to make, and I'm focusing on implementing features right now. Some people have been contributing their routes, but it also takes me a long time to review those.

At the same time, people want more available routes, faster.

As a solution, I've introduced the unofficial route system.

Routes found in the Routes directory are considered "official," and those in the UnofficialRoutes directory are "unofficial."

New route contributions will be added to the project unreviewed and bundled with releases, as unofficial routes. When loading a route, AugTyr will look for an unofficial route if an official one isn't found first.

*Saving is unchanged*. AugTyr will always save to the Routes directory.

## **0.3.1** Provided routes

 Official            | Official               | | Unofficial
:--------------------|:-----------------------|-|:-------------------
 Black Citadel       | Lion's Arch            | | Auric Basin
 Bloodtide Coast     | Lornar's Pass          | | Caledon Forest
 Brisban Wildlands   | Mount Maelstrom        | | Cursed Shore
 Diessa Plateau      | Queensdale             | | Dredgehaunt Cliffs
 Divinity's Reach    | Rata Sum               | | Frostgorge Sound
 Fields of Ruin      | Straits of Devastation | | Malchor's Leap
 Gendarran Fields    | The Grove              | | Metrica Province
 Harathi Hinterlands | Timberline Falls       | | Plains of Ashford
 Hoelbrak            | Wayfarer Foothills     | | Sparkfly Fen
 Kessex Hills        |                        | | Tangled Depths

## **0.3** User config

This version introduces a user config system, which lets you change many ways in which AugTyr behaves.

The file can be found in AugTyr\_Data/StreamingAssets/UserConfig.json. It is a json file that can be modified with any text editor (notepad should do, really).

Note: You might notice the json file contains C and C++ style comments. These are *technically* not allowed by the json specification, but Json.NET seems to be OK with them, so no problem for us.

There is no UI to change the user config. I *might* make one (separate from AugTyr itself) in the future.

Below is an overview of currently available options.

### **0.3.1** Display options

* `ScreenWidth` (int) Set to a positive number to have AugTyr use it as the desired resolution width.

* `ScreenHeight` (int) Set to a positive number to have AugTyr use it as the desired resolution height.

* `ByColorTransparency` (bool) Set to true to enable a different window transparency method. **Be warned**, it's slow and looks worse, so do not bother with this unless the default doesn't work for you.

### Behavior options

* `AutoUpdateGameDatabase` (bool) Set to false to disable game database updates. AugTyr should not establish any internet connection if this is the case, but in turn the game database (used to resolve waypoint names into waypoint codes) might become outdated.

* `StartInFollowMode` (bool) Set to true to instruct AugTyr to be in follow mode when started, instead of edit mode.

* `OrientationHelperDefault` (bool) Whether or not the orientation helper (the follow mode green line) should be on by default.

* `ConsoleFilter` (int) Possible values are:

  * `0` Show all messages.
 
  * `1` Filter *Info* messages.
 
  * `2` Filter *Info* and *Warning* messages.
 
  * `3` Filter all messages.

### Visual options

* `RouteWidth` (float) The width of all lines drawn, in all modes.

* `NodeSize` (float) The size of all nodes drawn, in all modes. Set this to 1 to use old size from v0.1.

### Input options

The only input option is `InputGroups` (object), which in turn can contain the following options.

#### "Console"

Active at all times.

Action name | Default binding | Action
--:|:-:|:--
"ToggleHide" | Ctrl + Numpad . | Hide or show the console.
"Clear" | Ctrl + Numpad 0 | Clear all messages from the console.

#### "Route"

Active at all times.

Action name | Default binding | Action
--:|:-:|:--
"Save" | Numpad * | Save the route with the ID it was loaded as.
"Load" | Numpad / | Load the route corresponding to the ID of the map you're currently on.
"LoadClipbardId" | Ctrl + Numpad / | Check the clipbard for a number, then load the route with that ID.
"ToggleMode" | Numpad . | Go from edit mode to follow mode or vice-versa.

#### "EditMode"

Active only in edit mode.

Action name | Default binding | Action
--:|:-:|:--
"AddNode" | Numpad + | Insert a node after the selected node at your current position.
"RemoveNode" | Numpad - | Remove the selected node.
"SelectClosestNode" | Numpad 5 | Select the node closest to you.
"SelectPreviousNode" | Numpad 4 | Select the previous node.
"SelectNextNode" | Numpad 6 | Select the next node.
"SelectFirstNode" | Ctrl + Numpad 4 | Select the first node.
"SelectLastNode" | Ctrl + Numpad 6 | Select the last node.
"PreviousNodeType" | Numpad 8 | Scroll the selected node's type backwards (see node types above).
"NextNodeType" | Numpad 2 | Scroll the selected node's type forwards (see node types above).
"MoveSelectedNode" | Numpad 7 | Move the selected node to your location.
"ToggleAttachNode" | Numpad 9 | Attach or detach the selected node. When attaching a node, it'll be inserted right after the latest selected attached node.
"GetNodeText" | Numpad 1 | Copy the selected node's text to your clipboard.
"SetNodeText" | Ctrl + Numpad 1 | Paste your clipboard text into the selected node's text.
"GetNodeData" | Numpad 3 | Copy the selected node's special text to your clipboard.
"SetNodeData" | Ctrl + Numpad 3 | Paste your clipboard text into the selected node's special text.
"ToggleAttachSelection" | Numpad 0 | Switch selection between the last selected attached node and the last selected detached node.

#### "FollowMode"

Active only in follow mode.

Action name | Default binding | Action
--:|:-:|:--
"SelectClosestNode" | Numpad 5 | Select the closest node to you.
"SelectPreviousNode" | Numpad 4 | Select the previous node.
"SelectNextNode" | Numpad 6 | Consume the selected node, selecting the next one.
"ToggleOrientationHelper" | Numpad 0 | Toggle the orientation helper.

### Input option format

Each input group contains a list of input actions. An input action is an object with the following elements:

* `ActionName` (string) The name of the action. Possibilities are as seen above.

* `KeyName` (string) The name of the key that's bound to this action. All possible keys are listed at the beginning of the UserConfig.json file.

* `Control` (bool) Whether or not the Ctrl key should be required as a part of the input. This element can be omited to use the default value of false.

Each action can be bound multiple times, or not at all. Additionally, binding multiple actions to the same input should work as expected, but produces no warning although it probably should; such a warning might be added in the future.

## Follow mode tips

* In this mode you will "consume" the selected node if you get close enough to it. Then, the next node will become selected.

* Consuming a teleport node with a waypoint code will automatically copy the code to your clipboard.

* If you miss a node that you don't think was critical, you don't have to go back for it. Just press 6.

* If you miss multiple nodes, you can always press 5 to select the closest node. If you think the closest node might actually already be behind you, you can press 5 and then 6 in quick succession.

## FAQ

### Why does AugTyr connect to the internet?

AugTyr uses the [official GW2 HTTPS API](https://wiki.guildwars2.com/wiki/API:Main) to build a "game database" of sorts, to be used to resolve waypoint names into waypoint codes.

The download already comes with an up-to-date built database, but AugTyr still makes one API call to check if there has been an update that could've made the database outdated.

All HTTPS API calls can be disabled by setting the `AutoUpdateGameDatabase` option to false.

### How can I tell the beginning of a route?

For now there's two options:

1. Select the first node (Ctrl + Numpad 4 by default in edit mode), then change to follow mode. The orientation helper will point you in the direction of the first node, try to figure it out based on that.

2. Since most routes are just 3D versions of Lulle's routes, you check the route for the map you want beforehand [here](https://oopsy.enjin.com/forum/m/41271713/viewthread/28848825-law-lulles-advanced-worldcompletion-guide).

A better solution will be integrated into AugTyr in the future.

### AugTyr's resolution is wrong

You can set the resolution you want explicitly in the user config file.

### I only see a black screen

This has been reported to happen if Windows Aero is disabled. Try enabling it.

If that doesn't help, you can try the by color transparency mode. It's usually really slow, but it might fix the problem. Set the option to true in the user config file.

## Download

[AugTyr-0.3.1](https://github.com/ideka/AugTyr/releases/tag/v0.3.1-beta)
