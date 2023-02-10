using Unity.Networking.Transport;

public class NetKeepAlive : NetMessage
{
    public NetKeepAlive() // <-- Making the  box
    {
        Code = OpCode.KEEP_ALIVE;
    }

    public NetKeepAlive(DataStreamReader reader) //<-- Recieving the box
    {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }
    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public override void Deserialize(DataStreamReader reader)
    {

    }
    public override void RecievedOnClient()
    {
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }
    public override void RecievedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }
}
