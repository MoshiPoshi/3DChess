using UnityEngine;
using Unity.Networking.Transport;
using System;

public class NetMessage
{
    public OpCode Code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public virtual void Deserialize(DataStreamReader reader)
    {
        
    }
    public virtual void RecievedOnClient()
    {

    }
    public virtual void RecievedOnServer(NetworkConnection cnn)
    {

    }
}
