# 7dtd-ServerTools with /return function
This is a branch of Server Tools v2.2  by Dmustanger<br>
Go to https://github.com/dmustanger/7dtd-ServerTools/releases for the original files<br>

This mod changes the original ServerTools.dll by adding the following commands:<br>
/setreturn /delreturn /return <br>
These will allow the player to set and delete a return teleport point in the world (or a second home).<br>

#If upgrading from the original Server Tools v2.2

1.)Stop the server<br>
2.)Download and extract the files.<br>
3.)Replace ServerTools.dll in the Mods folder on the server with the one you just extraced.<br>
2.)Delete root/saves/ServerTools/ServerToolsConfig.xml<br>
		-or to save your current settings-<br>
   Add the following line under the sethome settings. Set the delay to be the same as sethome (can be different).<br>
		<Tool Name="SetReturn" Enable="True" DelayBetweenSetReturnUses="0" /><br>
3.)Delete root/saves/ServerTools/ServerToolsPhrases.xml<br>

# New Installation

Download and extract the files.<br>
Copy the Server Tools folder to the Mods folder on the root directory of your server.<br>
Start the server. The mod will auto create the config file in the game save directory. Enable each part of the mod you want via ..\your game save directory\ServerTools\ServerToolsConfig.xml<br>
Once a module is enabled if it has a config it will auto create them in the ServerTools folder.<br>

Enjoy!

# Original Features of Server Tools v2.2
Custom chat commands with custom color. Add your own commands via config.<br>
/Gimme with adjustable timer and items via config.<br>
/Killme with adjustable timer via config.<br>
/home /sethome /delhome with adjustable timer for /home via config.<br>
High ping kicker with ping immunity. Can add players to the immunity list via config or console command.<br>
Ban or kick players for invalid items/blocks in their inventory. Chose what items/blocks are invalid via config.<br>
Alert players of Invalid Item stack numbers in their inventory.<br>
Chat logger. Saves all ingame chat to a log file in the game save directory.<br>
Bad word filter. Replaces bad words with "*****". Can add bad words via config.<br>
Motd adjustable via config.<br>
InfoTicker/Scrolling messages adjustable via config.<br>
Auto save the world every x amount of minutes adjustable via config.<br>
