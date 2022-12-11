# Network Assignment: Rigidbody Interpolation

![gif-network-homework](https://user-images.githubusercontent.com/32450751/206931226-535f823e-598f-4e7e-bf45-339a790834e4.gif)

 Network project that uses Unity & Photon.
 
 The projets uses Unity and Photon for the sync of a complex scene. 
 
Two instances of the game are expected to connect to the same Photon server. The first instance to be executed and to connect to the server becomes the master client. The master client creates a room on the photon server and sends position and rotation of each (moving) cube to the server. Any other instance of the game will become a "hearing" client, that is, it will be able to receve the position and rotation information of the cubes and place them accordingly. To achieve realtime sync of so many objects, compression is applied.

You can find a build on itch: https://aramas.itch.io/network-transform-sync-test
