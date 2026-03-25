using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;


public struct PlayerInput : INetworkSerializable
{
    public uint tick;
    public float moveY;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref moveY);
    }
}

