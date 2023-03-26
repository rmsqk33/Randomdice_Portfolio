using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System;
using UnityEngine;
using Packet;
using System.Linq;

public class FServerManager : FSingleton<FServerManager>
{
    const string SERVER_IP = "127.0.0.1";
    const int SERVER_PORT = 7777;
    const int PACKET_MAX = 10240;

    private TcpClient tcpClient = null;
    private NetworkStream netStream = null;
    private Thread receiveMessageThread = null;
    public bool IsConnectedServer { get; private set; } = false;

    public delegate void PacketHandler(in byte[] InBuffer);

    private Dictionary<PacketType, PacketHandler> packetHandlerMap = new Dictionary<PacketType, PacketHandler>();

    struct MessageData
    {
        public PacketType type;
        public byte[] buffer;
    }
    private List<MessageData> messageQueue = new List<MessageData>();

    void Update()
    {
        ExecuteMessage();
    }

    void ExecuteMessage()
    {
        if (messageQueue.Count == 0)
            return;

        MessageData messageData = messageQueue[0];
        messageQueue.RemoveAt(0);

        if (!packetHandlerMap.ContainsKey(messageData.type))
            return;

        packetHandlerMap[messageData.type](messageData.buffer);
    }

    void OnApplicationQuit()
    {
        DisconnectServer();
    }

    void DisconnectServer()
    {
        if(IsConnectedServer)
        {
            IsConnectedServer = false;

            if(receiveMessageThread != null)
                receiveMessageThread.Join();

            if(netStream != null)
                netStream.Close();

            if(tcpClient != null)
                tcpClient.Close();
        }
    }

    public bool ConnectServer()
    {
        if (IsConnectedServer)
            return false;

        try
        {
            tcpClient = new TcpClient(SERVER_IP, SERVER_PORT);
            netStream = tcpClient.GetStream();

            receiveMessageThread = new Thread(ReceiveMessage);
            receiveMessageThread.Start();

            IsConnectedServer = true;
        }
        catch (Exception e)
        {
            Debug.Log("Server Connect Fail: " + e);
            return false;
        }

        return true;
    }

    async void ReceiveMessage()
    {
        byte[] buffer = new byte[PACKET_MAX];
        int packetSize = 0;
        int readSize = 0;

        while (IsConnectedServer)
        {
            readSize += await netStream.ReadAsync(buffer, readSize, PACKET_MAX - readSize);

            // 한번에 여러 패킷이 오는 경우를 대비하여 반복처리
            while (0 < readSize)
            {
                // 처음 패킷을 받은 경우 패킷 사이즈 저장
                if (packetSize == 0)
                    packetSize = BitConverter.ToInt32(buffer, 0);

                // 패킷 전체를 전달받지 못한 경우 계속 데이터를 받도록 한다.
                if (readSize < packetSize)
                    break;

                // 패킷 전체를 전부 전달받으면 메시지 큐에 데이터를 담는다.
                MessageData messageData = new MessageData();
                messageData.type = (PacketType)BitConverter.ToInt32(buffer, sizeof(int));
                messageData.buffer = new byte[packetSize];

                int commonDataLength = sizeof(int) + sizeof(PacketType);
                Array.Copy(buffer, commonDataLength, messageData.buffer, 0, packetSize - commonDataLength);

                messageQueue.Add(messageData);

                readSize -= packetSize;

                // 처리 안된 패킷이 있으면 앞으로 땡기기
                if (0 < readSize)
                    Array.Copy(buffer, packetSize, buffer, 0, readSize);

                packetSize = 0;
            }
        }
    }

    public void SendMessage(in PacketBase InPacket)
    {
        if (netStream == null)
            return;

        try
        {
            if (netStream.CanWrite)
            {
                List<byte> buffer = new List<byte>();
                InPacket.Serialize(buffer);
                netStream.Write(buffer.ToArray(), 0, buffer.Count());
            }
        }
        catch (SocketException e)
        {
            Debug.Log("SendMessage Fail: " + e);
        }
    }

    public void AddPacketHandler(PacketType InType, PacketHandler InHandler)
    {
        packetHandlerMap.Add(InType, InHandler);
    }

}
