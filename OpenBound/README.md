<h2>Different things from the original game:</h2>

<h3>Assets</h3>

Reminder: The following clients were datamined in order to acquire all necessary assets. Because of that, newer assets/mobiles/features aren't going to be incorporated into the game.
- MagicPunk (Non-GB game made by the same company)
- Beta
- WC (Season 1) (North American/European/South American/Korean clients)
- Season 2 (North American/South American clients)

1. Audio
   - Movement - A few mobiles IN THE ORIGINAL GAME have no sound when moving or when are stuck. To address this (not) issue, I've decided to add placeholders from several other sources. Here is the complete list containing general information:

     - Mobiles that does not make sound when move:
       - Aduka [3]
       - J. Frog [3]
       - Kalsiddon [3]
       - Knight [1]
       - Turtle [1]

     - Mobiles that does not make sound when are stuck or out of movement left:
       - Aduka [3]
       - A. Sate [1]
       - Dragon [2]
       - Grub [2]
       - Ice [2]
       - JD [3]
       - J. Frog [3]
       - Kalsiddon [3]
       - Knight [1]
       - Turtle [1]

Legend:
[1] - Sound currently reproduced is a placeholder that comes from a different source (more details on Assets.3) 
[2] - Sound currently reproduced is a placeholder that comes from another sound with a different pitch to fool player's ears
[3] - All sounds played by this mobile are copied resources from other mobiles.

2. Assets that couldn't be extracted from the original game
   - Mobile
     - Dragon's Fireball (S1/S2)

   - Audio
     - Trico's unable to move sound (if it has any)

3. Current Project's Placeholder Assets:
   - Mobiles
     - Knight
       - Moving - Gallop sound extracted from HoMM3
       - UnableToMove - Horse neigh sound extracted from HoMM3

<h3>General</h3>

1. Audio
   - Music player randomizes songs from every GB version
   - Music supports transition effects. The music fades out/in some scenes, the "original" music sudden change was kept for a few exceptions.
   - As explained in Assets.1.1, some mobiles were suposed to make no sound. However, a placeholder is being played.

2. Mobiles
   - There is no restrictions concerning mobiles. Dragon and Knight are now available to pick.
   - Dragon, Knight and Lightning were slightly changed in order to make'em balanced. More information can be seen in General.7

3. Game Mechanics
   - The server can broadcast the number of expected servers.
   - The 'unable to move' animation is now shared among players in the same room.
   - There is no 'Leave Match' button anymore. If the user wants to leave the match he must close the game.

4. Interface
   - There are blended interface elements between S1 and S2 clients. Mobile's picklist for instance.
   - The InGame/Wind Compass has a richer animation than the original client.
   - Incoming weather system has also a improved animation.
   - The incoming weather pointer shown atop of the screen can predict all incoming weather shown on the IncomingWeather menu. They also have a fade-in animation.
   - Some weather effects (tornado/force/mirror...) have a "infinite-scrolling" animation to make it look more "animated".
   - Each player has a UNIQUE (or maybe not) chat color depending on the given nickname.
   - Server message icons are no longer images, but glyphs from FontAwesome.

5. Technical Information
   - The resolution can be chanced. However, resolutions that surpass the stage's foreground/background sprite size can break the camera and the parallaxing. Keep in mind that there is no solution to that since it is limited by its sprites dimensions.
   - FPS is locked at 60 while the original game had lock at 30. The animations aren't rich enough to sustain bigger refresh rates.
   - The original game only supported the 800x600 resolution, hence, all the menus sprites were created for this resolution. In order to adapt the menus for bigger resolutions, I've scaled up all interface dynamically.
   - The game can be opened at window mode.
   - This game is built on monogame, meaning that it can be ported to mobile devices or other OS.

6. Networking
   - All clients are fed with the same information at the same time, meaning that it is expected to have no delay between clients.

7. Mobile Alterations
  - Dragon
    - SS was altered to make it more useful. Now there are different freezetimes for each summoned drake.
  - Lightning
    - Lightning shots works like the season 2 GB. The main projectile creates erosion and deals a fraction of the total damage, while the electricity/discharge deals extra damage.
  - [Aesthetic] Various
    - All mobile's projectile that uses Trace or HelicoidalTrace (Mage/Turtle S1, S2) looks different, even if the sprites are the same as the ones used in the original game. I intend to address this issue later.
  
<h3>Known bugs</h3>

1. Client
   - It is possible to freeze the match by passing the turn (there is a button on the interface for that).
   - The mobiles are unbalanced (INTENDED)
   - [Testing] A few mobile projectiles and mobile animations are unsynched.
   - Weather animations are not fps proof. A laggy player can have different wind settings.
   - Maximum resolution can not be greater than 1600 width OR 1600 height.

2. Server
   - There is no verification check on teams equality when a game is being started (INTENDED).
   - All servers are not smart enough to "re-attatch" the connections. In order to make it work again close it and open again in the same order described in Configurations & Setup.