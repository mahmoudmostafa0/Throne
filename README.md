Throne
===============================================

Conquer Online emulator in development.

Written in C#, currently targeted at the patches of late 2014. 
The goal of the Throne project aims to create a stable server platform for Conquer Online.

Currently, I would call the project confused. There are, in a few cases.. different implementations of the same thing. 
I'm still learning new-to-me, better, cooler ways to do things in C#, so the project isn't pure as I would like.

Development was originally Mono, but moved to Visual Studio. The project could be ported back to Mono with some effort.
******************************

News
----
***December 12th, 2014*** Throne r38 released.


Features
--------
 - Split server... Login and World
 - C# Scripting (Scripts are linked into their own solutions)
 - Plenty of confusing things (I'm learning... I couldn't find everything I did wrong)
 - Global OOP based commands (same commands can be used on console and ingame)
 - "Robust" base. At least I think so:
   - Fluent NHibernate
   - Asyncronous Sockets
   - WCF IPC
   - Actor based threading (one thread per core, one thread per actor.. lightest load thread after idle, load balanced)
 - Conquer Implementations:
   - Roles
   - Maps
   - Items (equip, inventory, map)
   - Movement
   - Limited chat support
   - Users can see others (screening works)

Getting Started
----
 - Create the database
   - Creating the database in Throne is easy. First, put your DB server information in the .settings files which you'll find    in the solutions's Properties folder. You can use MSSQL, MySQL, pretty much whatever is handy, just make sure you make the    proper connection string modifications. Create a new database for throne, you can name it whatever, just make sure you       modify the database info in both servers. Once you build the project and get it running, on the console of both servers,     type "createdb"
 - Configure IPs and ports
   - Get back into those global settings files for both servers, modify the IPs to whatever your router or public IP is, set your ports.. the default ports should work, just make sure you use the same ports in your loader.
 - Prepare your client
   - No loader is included with the project yet, InfamousNoone released a good one in [ConquerServerV3](https://www.assembla.com/code/conquerserverv3/subversion/nodes) 
 - Login!



![Throne in action...](http://i.imgur.com/tZs4aeu.jpg)



Credits/Contributers/References
-------
- Mentalis/sbradno
- CptSky
- Spirited Fang
- Impulse
- Smallxmac
- InfamousNoone
- Encore

Leave a message on [Chained2PVP](http://chained2pvp.com/topic/332-throne-project-development/) if you belong in the credits!



