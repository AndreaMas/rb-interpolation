# Network Assignment: Rigidbody Interpolation

![gif-network-homework](https://user-images.githubusercontent.com/32450751/206931226-535f823e-598f-4e7e-bf45-339a790834e4.gif)

## What is this?

A Network project. The projets uses Unity and Photon for the sync of a complex scene.

## How does it work?
 
Two instances of the game are expected to be exectued and connect to the same Photon server. The first instance to connect becomes the master client. The master client creates a room on the server and streams position and rotation of each (moving) cube to the server. The other instance of the game, upon connection to the server, will become a "hearing" client, that is, it will be able to receve the position and rotation information of the cubes. To achieve realtime sync of so many objects, compression is applied, and smoothing/interpolation on the client side is performed.

## Known issues

The rotation interpolation is not perfect. Sometimes (can be seen in the gif) the red cube jumps between two different rotation positions, almost as if it receves an ambiguous rotation. Surely I messed up some quaternion stuff.

## Links

Build on itch: https://aramas.itch.io/network-transform-sync-test
