using UnityEngine;
using Unity.Netcode;

public struct BallState : INetworkSerializable
{
    public uint tick;
    public Vector2 position;
    public Vector2 velocity;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref velocity);
    }
}