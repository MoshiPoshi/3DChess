using System;
using Unity.Networking.Transport;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class Server : MonoBehaviour
{
    #region Singleton implementation
    public static Server Instance { set; get; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    private bool isActive = false;
    private const float keepAliveTickRate = 20.0f;
    private float lastkeepAlive;

    public Action connectionDropped;

    //Methods
    public void Init(ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endPoint = NetworkEndPoint.AnyIpv4;
        endPoint.Port = port;

        if (driver.Bind(endPoint) != 0)
        {
            Debug.Log("Unable to bind on port " + endPoint.Port);
            return;
        }
        else
        {
            driver.Listen();
            Debug.Log("Currently listening on port " + endPoint.Port);
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
        isActive = true;
    }
    public void Shutdown()
    {
        if (isActive)
        {
            driver.Dispose();
            connections.Dispose();
            isActive = false;
        }
    }
    public void OnDestroy()
    {
        Shutdown();
    }

    public void Update()
    {
        if (!isActive)
            return;

        KeepAlive();

        driver.ScheduleUpdate().Complete();

        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();
    }
    private void KeepAlive()
    {
        if (Time.time -lastkeepAlive > keepAliveTickRate)
        {
            lastkeepAlive = Time.time;
            Broadcast(new NetKeepAlive());
        }
    }
    private void CleanupConnections()
    {
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                --i;
            }            
        }
    }
    private void AcceptNewConnections()
    {
        // Accept new connections
        NetworkConnection c;
        while ((c = driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c);
        }
    }
    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        for (int i = 0; i < connections.Length; i++)
        {
            NetworkEvent.Type cmd;
            while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, connections[i], this);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown(); // because we're in a two players game
                }
            }
        }
    }

    //server specific
    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);
    }
    public void Broadcast(NetMessage msg)
    {
        for (int i = 0; i < connections.Length; i++)
            if (connections[i].IsCreated)
            {
                //Debug.Log($"Sneding {msg.code} to : {connections[i].InternalId}")
                SendToClient(connections[i], msg);
            }
    }

}
