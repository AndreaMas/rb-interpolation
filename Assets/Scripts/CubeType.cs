using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using Photon.Pun.UtilityScripts;
using ExitGames.Client.Photon;

public class CubeType
{
    public short cubeId; // if negative cube is lit/touched
    short posX;
    byte posY;
    short posZ;
    byte rotX;
    byte rotY;
    byte rotZ; // W is reconstructable

    // Hardcoded size of class
    public static ushort sizeBytes = 2 +             // id & lit
                                 2 + 1 + 2 +         // position
                                 1 + 1 + 1;          // rotation

    // Utility functions

    static short BytesToShort(ushort byte1, ushort byte2)
    {
        return (short)((byte2 << 8) | byte1);
    }

    static void BytesFromShort(short number, out byte byte1, out byte byte2)
    {
        byte2 = (byte)(number >> 8);
        byte1 = (byte)(number & 255);
    }

    // Remaps value from range [aMin, aMax] to range [bMin, bMax].
    static float Remap(float value, float aMin, float aMax, float bMin, float bMax)
    {
        float t = Mathf.InverseLerp(aMin, aMax, value);
        return Mathf.Lerp(bMin, bMax, t);
    }

    public void Compress(Vector3 position, Quaternion rotation, short id, bool touched)
    {
        // Cubes live in the volume:
        // x = [-25, 25]
        // y = [-2, 2]
        // z = [-25, 25]

        cubeId = (!touched) ? id : (short)(-id); // cube Id negative if cube is touched/lit (not ideal ...)

        posX = (short)Mathf.Round(Remap(position.x, -25.0f, 25.0f, short.MinValue, short.MaxValue));
        posY = (byte)Mathf.Round(Remap(position.y, -2.0f, 2.0f, byte.MinValue, byte.MaxValue));
        posZ = (short)Mathf.Round(Remap(position.z, -25.0f, 25.0f, short.MinValue, short.MaxValue));

        // since (x,y,z,w) = (-x,-y,-z,-w), we always send the quaternion with w >= 0
        int equivalentQuatCoeff = (rotation.w >= 0) ? 1 : -1;
        rotX = (byte)Mathf.Round(Remap(equivalentQuatCoeff * rotation.x, -1, 1, byte.MinValue, byte.MaxValue));
        rotY = (byte)Mathf.Round(Remap(equivalentQuatCoeff * rotation.y, -1, 1, byte.MinValue, byte.MaxValue));
        rotZ = (byte)Mathf.Round(Remap(equivalentQuatCoeff * rotation.z, -1, 1, byte.MinValue, byte.MaxValue));
    }

    public void Decompress(ref Vector3 position, ref Quaternion rotation)
    {
        position.x = Remap(posX, short.MinValue, short.MaxValue, -25.0f, 25.0f);
        position.y = Remap(posY, byte.MinValue, byte.MaxValue, -2.0f, 2.0f);
        position.z = Remap(posZ, short.MinValue, short.MaxValue, -25.0f, 25.0f);

        rotation.x = Remap(rotX, 0, 255, -1, 1);
        rotation.y = Remap(rotY, 0, 255, -1, 1);
        rotation.z = Remap(rotZ, 0, 255, -1, 1);
        rotation.w = Mathf.Sqrt(1 - (rotation.x * rotation.x + rotation.y * rotation.y + rotation.z * rotation.z));
    }

    public static byte[] Serialize(object customobject)
    {
        var co = (CubeType)customobject;
        byte[] bytes = new byte[sizeBytes];

        // Protocol.Serialize works only with short/int/float.
        // bundle bytes in groups of two to make shorts.
        short bundlePosYandRotX = BytesToShort(co.posY, co.rotX);
        short bundleRotYandRotZ = BytesToShort(co.rotY, co.rotZ);

        int index = 0;
        Protocol.Serialize(co.cubeId, bytes, ref index);
        Protocol.Serialize(co.posX, bytes, ref index);
        Protocol.Serialize(co.posZ, bytes, ref index);
        Protocol.Serialize(bundlePosYandRotX, bytes, ref index);
        Protocol.Serialize(bundleRotYandRotZ, bytes, ref index);

        return bytes;
    }

    public static object Deserialize(byte[] data)
    {
        var result = new CubeType();

        short bundlePosYandRotX;
        short bundleRotYandRotZ;

        int index = 0;
        Protocol.Deserialize(out result.cubeId, data, ref index);
        Protocol.Deserialize(out result.posX, data, ref index);
        Protocol.Deserialize(out result.posZ, data, ref index);
        Protocol.Deserialize(out bundlePosYandRotX, data, ref index);
        Protocol.Deserialize(out bundleRotYandRotZ, data, ref index);


        BytesFromShort(bundlePosYandRotX, out result.posY, out result.rotX);
        BytesFromShort(bundleRotYandRotZ, out result.rotY, out result.rotZ);

        return result;
    }

}
