using Unity.Networking.Transport;

public class NetRematch : NetMessage
{
    public int teamId;
    public byte wantRematch;

    public NetRematch() // <-- Making the  box
    {
        Code = OpCode.REMATCH;
    }
    public NetRematch(DataStreamReader reader) //<-- Recieving the box
    {
        Code = OpCode.REMATCH;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamId);
        writer.WriteByte(wantRematch);
    }
    public override void Deserialize(DataStreamReader reader)
    {
        teamId = reader.ReadInt();
        wantRematch = reader.ReadByte();
    }
    public override void RecievedOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_REMATCH?.Invoke(this, cnn);
    }
}
