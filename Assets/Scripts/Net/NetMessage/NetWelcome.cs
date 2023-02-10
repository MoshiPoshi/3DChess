using UnityEngine;
using Unity.Networking.Transport;
using System;

public class NetWelcome : NetMessage
{
    public int AssignedTeam { set; get; }

    public NetWelcome()
    {
        Code = OpCode.WELCOME;
    }
    public NetWelcome(DataStreamReader reader)
    {
        Code = OpCode.WELCOME;
        Deserialize(reader);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(AssignedTeam);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        // We already read the byte in the NetUtility::onData
        AssignedTeam = reader.ReadInt();
    } 
    public override void RecievedOnClient()
    {
        NetUtility.C_WELCOME?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }
}
