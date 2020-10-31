<h2>Configuration & Setup</h2>

This solution contains four applications that works together to make the game work.

1. Avatar API
2. OpenBound Asset Tools
3. OpenBound Game Launcher
4. OpenBound (Game Application)
5. OpenBound Login Server
6. OpenBound Lobby Server
7. OpenBound Game Server

Remember to build'em all beforehand.

In the current version is MANDATORY that all servers are opened in the same order as I have mentioned above.

All the necessary configuration (IP tables, public/local IPs and database address) can be edited by changing the configuration files that each application creates when are opened.

<h3>Configuring the Solution</h3>

<h2>Avatar API</h2>

Avatar API includes easy and understandable .NET CORE implementations for PayPal and Stripe, two payment gateways. It is already configured and integrated with the database. It is easy to use and all configuration instruction lies whithin AvatarAPI readme.md.

<h2>Game Application</h2>

OpenBound Game requires an texture pack in order to be built. Since all the textures belongs to it's owners, I can not share it in here. If you want to know more info about it, feel free to join my discord channel here: https://discord.gg/rVnAb5t

Assuming you have a texture pack in your machine, all you have to do is unzip it on OpenBound/Content folder. After that, your folder structure is going to look like this:
- OpenBound
   - Content
      - Audio
      - Debug
      - Fonts
      - ...
      - Content.mgcb

<h2>Game Servers</h2>

1. First of all, you have to generate the config files, for that, all you have to do is open the LoginServer, LobbyServer and GameServer executables at least once and edit the generated .txt files.

The default paths for the configuration files are:

- Login Server/Config
- Lobby Server/Config
- Game Server/Config

Further instructions on how to proceed and a few specifics about configurations are written on the default configuration .txt files.
In case you have made a mess on any file, delete it and let the application create new ones!

Database connections are handled by Entity Framework. In case you prefer a free database, such as MySQL, you have to do is download It's driver and change the contexts on
Openbound Network Object/OpenboundDatabaseContext. But it would require a few programming understanding.

2. Configure your database custom user to have admin privilleges. It is mandatory because of the EntityFramework's (flawed?) design.

This application creates the database by itself as long as the entered configurations are correct. Here is a checklist of things that you must do to ensure the server will work:
   - Check if the address is valid.
   - Check if the database is connectable.
   - Check if the database instance is running. 
   - Check if the user is valid and has admin privilleges.

The easiest way to check these things is to connect using VS's Server Explorer, SQL Server Management Studio or any other database interface and then perform some quick operations
like creating and then dropping a table on master using your newly creted user.

Currently compatible databases:

- SQL Server LocalDB (Installed automatically with VS Community 2019 or superior).
- SQL Server Express
- SQL Server (Recommended)

<h2>Project Information</h2>

There is a README.md explaining details for each kind of project.

This project is divided in other 7 sub-projects. Here is a VERY resumed list of each project's goal.

For further information read the README.md inside each project directory.

 - GunboundImageFix - A set of tools for converting from encrypted & proprietary formats to PNG spritesheets. And other misc helpers used througout the project development.
 - GunboundImageProcessing - Image library for trivial image processing like resizing, pixel comparison, conversion between matrixes and bitmaps, grayscales, a few filters and so on. This is mostly used as a helper/testing functions for GunboundImageFix.
 - OpenBound - The game client itself.
 - Openbound Game Server
 - Openbound Login Server
 - Openbound Lobby Server
 - Network Object Library - Object library used in one or more game-related sub-projects.

<h2>Contributors</h2>

<h3>Developers</h3>

 - [WickedPeanuts](https://github.com/WickedPeanuts/) (Carlos Henrique)
 - [Icy Willow](https://github.com/IcyWillow/) (Vinícius Pontes) - Since 05/21/2020

<h3>Bug Reports/Testers</h3>

 - [sdyalor](https://github.com/sdyalor)

<h3>Game client assets/Datamined values</h3>

 - ursoGb

<h2>Licence and copyright</h2>

All repositories/programs that copies this repository must include the following line:
- Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>

Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>

This file is part of OpenBound.

OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
