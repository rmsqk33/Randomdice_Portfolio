using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using UnityEngine;
using Packet;
using System.Linq;
using Unity.VisualScripting;

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

            // ????????? ?????? ????????? ?????? ????????? ???????????? ????????????
            while (0 < readSize)
            {
                // ?????? ????????? ?????? ?????? ?????? ????????? ??????
                if (packetSize == 0)
                    packetSize = BitConverter.ToInt32(buffer, 0);

                // ?????? ????????? ???????????? ?????? ?????? ?????? ???????????? ????????? ??????.
                if (readSize < packetSize)
                    break;

                // ?????? ????????? ?????? ??????????????? ????????? ?????? ???????????? ?????????.
                MessageData messageData = new MessageData();
                messageData.type = (PacketType)BitConverter.ToInt32(buffer, sizeof(int));
                messageData.buffer = new byte[packetSize];

                int commonDataLength = sizeof(int) + sizeof(PacketType);
                Array.Copy(buffer, commonDataLength, messageData.buffer, 0, packetSize - commonDataLength);

                messageQueue.Add(messageData);

                readSize -= packetSize;

                // ?????? ?????? ????????? ????????? ????????? ?????????
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
