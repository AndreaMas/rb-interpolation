# Network Assignment: Rigidbody Interpolation

![gif-network-homework](https://user-images.githubusercontent.com/32450751/206931226-535f823e-598f-4e7e-bf45-339a790834e4.gif)

## What is this?

A Network project. The projets uses Unity and Photon for the sync of a complex scene.

## How does it work?
 
Two instances of the game are expected to connect to the same Photon server. The first instance to be executed and to connect to the server becomes the master client. The master client creates a room on the photon server and sends position and rotation of each (moving) cube to the server. Any other instance of the game will become a "hearing" client, that is, it will be able to receve the position and rotation information of the cubes and place them accordingly. To achieve realtime sync of so many objects, compression is applied, and smoothing/interpolation on the client side is performed.

## Links

Build on itch: https://aramas.itch.io/network-transform-sync-test
