Dialogue Grammar
===
This file specifies how text files that contain information about an NPC's interaction with the player must be written.

Grammar for NPC line, Player Response
---
The grammar for one line (or "exchange") of dialogue is:

\[_NPC Quote_ \{_Events_\}\] \[_Player Response Option 1_ \{_Events_\}\] \[_Player Response Option 2_ \{_Events_\}\] ...

_NPC Quote_ specifies what the NPC will say. _NPC Quote_ must be a string that does not contain square brackets (\[\]) or curly braces (\{\}).

_Player Response Option X_ specifies a line the player can select in response to _NPC Quote_. There can be as many options as desired (including 0) -- generally, though, options should be limited to 4 or less to keep the dialogue uncluttered. Options will be listed in the order in which they are specified in the file. _Player Response Option X_ must be a string that does not contain square brackets (\[\]) or curly braces(\{\}).

Grammar for Events
---
In the above specification there is \{_Events_\} next to each line. _Events_ is a set of descriptions that specify where to go next in the dialogue and how to affect the game state. The set can have no elements, or multiple -- different elements must by comma (,) delimited. The list of possible events follows below:

* **End** - specifies that the conversation will exit
* **Line _X_** - specifies to go to line _X_ in the dialogue, where _X_ is a positive integer. When the file is parsed from top to bottom each line is implicitly numbered in ascending order, starting at one.
* **Set _XXX_** - specifies a game constant to be "on"; this constant can be checked for future dialogue. _XXX_ will be some unique string of characters (no spaces or symbols) to identify the constant.
* **Event _XXX_** - specifies a game specific event that must be programmed (for example, getting an item). _XXX_ will be some unique string of characters (no spaces or symbols) to identify that event. Since the game won't have a lot of these special cases, it's not inefficient to implement it this way.

Control Paths
---
To allow for more sophisticated interactions, where the NPC isn't always saying the same thing when you talk to him, we can use control paths (special keywords) before lines of dialogue to specify where the conversation will start. A list of keywords follows below:

* **if _XXX_** - specifies that the line below will be executed first if _XXX_ is fulfilled. _XXX_ will be some unique string of characters (no spaces or symbols) to identify that condition. If there are multiple conditionals in a file then they will be checked top to bottom -- the first one that is true will be chosen.
* **choose _X_,_Y_** - specifies that there is an equal probability of selecting any one line between lines _X_ and _Y_ (inclusive) first. Would use for characters who say random stuff.

NPC Name
---
The name of the text file (before the first period (.)) will specify the name of the NPC. Don't use any fancy names with symbols.

Samples
---
Refer to separate .txt files in the same directory.