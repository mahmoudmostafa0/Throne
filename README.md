Throne
===============================================

Conquer Online emulator in development.

Written in C#, currently targeted at the patches of mid January 2015. 
The goal of the Throne project aims to create a stable server platform for Conquer Online.

Development was originally with Mono, but moved to Visual Studio. The project could be ported back to Mono with some effort.
******************************

News
----
***December 12th, 2014*** Throne r38 released.


Features
--------
 - Split server... Login and World
 - C# Scripting (Scripts are linked into their own solutions)
 - Global OOP based commands (same commands can be used on console and ingame)
 - Extensive base library
   - Fluent NHibernate
   - Asyncronous Sockets
   - WCF IPC
   - Actor based threading (one thread per core, one thread per actor, load balanced)
 - Conquer Implementations:
   - Roles
   - Maps
   - Items (equip, inventory, map)
   - Movement
   - Limited chat support
   - Users can see others (screening works)
   - Limited NPC support (async dialogs, support for script hooking incoming)
   - Inbox and mail

Getting Started
----
 - Create the database
   - Creating the database in Throne is easy. First, put your DB server information in the .settings files which you'll find    in the solutions's Properties folder. You can use MSSQL, MySQL, pretty much whatever is handy, just make sure you make the    proper connection string modifications. Create a new database for throne, you can name it whatever, just make sure you       modify the database info in both servers. Once you build the project and get it running, on the console of both servers,     type "createdb"
 - Configure IPs and ports
   - Get back into those global settings files for both servers, modify the IPs to whatever your router or public IP is, set your ports.. the default ports should work, just make sure you use the same ports in your loader.
 - Prepare a v6020+ client
   - No loader is included with the project yet, InfamousNoone released a good one in [ConquerServerV3](https://www.assembla.com/code/conquerserverv3/subversion/nodes).. 
 - Test
   - For commands, type help on the console or >help ingame. Standard command usage: >teleport 1002 300 278
   - For targeted commands, use >>(command) (target). Usage: >>teleport Person 1002 300 378



![Throne in action...](http://i.imgur.com/tZs4aeu.jpg)



Credits/Contributers/References
-------
- Mentalis/sbradno
- CptSky
- Spirited Fang
- Korvacs
- Impulse
- Smallxmac
- InfamousNoone
- Encore
- Anthrax

Leave a message on [Chained2PVP](http://chained2pvp.com/topic/332-throne-project-development/) or [ElitePVPers](http://www.elitepvpers.com/forum/co2-pserver-guides-releases/3526603-release-project-development-throne.html) if you wish to contribute to the project or if you belong in the credits.





