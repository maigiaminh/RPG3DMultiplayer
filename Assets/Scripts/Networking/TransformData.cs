using Unity.Netcode;
using UnityEngine;

public class TransformData : INetworkSerializable
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref Rotation);
        serializer.SerializeValue(ref Scale);
    }

}
