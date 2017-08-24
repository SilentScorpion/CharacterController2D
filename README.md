# CharacterController2D

A Controller that allows players to
1. Move
2. Jump
3. Climb up/down the slopes
4. Moving platforms
5. Jump on the walls

Controller2D.cs does all the calculation for collision detection using Raycasting.
Raycast Controller handles player movement, slopes and moving platforms far more consistently than Rigidbody.

Player.cs does takes input from the user and then sends the information to Controller2D.cs to move the Player.

Took help from Sebastian Lague's tutorials in this matter to achieve the same. 
Added double jump in the Player's armoury.
