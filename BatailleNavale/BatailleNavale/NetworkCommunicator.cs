using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using BatailleNavale.Model;
using Open.Nat;
using System.Threading;
using Newtonsoft.Json;
using BatailleNavale.Controller;

namespace BatailleNavale.Net
{
    public class NetworkCommunicator
    {
        public const ProtocolType PROTOCOL_TYPE = ProtocolType.Tcp;

        public int Ping { get; private set; }
        public string JoinPassword { get; set; }
        public NetworkPlayer RemotePlayer { get; set; }


        public event PlayerJoined PlayerJoinedEvent;
        public event PlayerLeft PlayerLeftEvent;
        public event PlayerReady PlayerReadyEvent;
        public event GameEnded GameEndedEvent;
        public event ChatMessageReceived ChatMessageReceivedEvent;
        public event EnemyHit EnemyHitEvent;
        public event IllegalHit IllegalHitEvent;

        public delegate void PlayerJoined();
        public delegate void PlayerLeft();
        public delegate void PlayerReady();
        public delegate void GameEnded(GameResult result);
        public delegate void ChatMessageReceived(string content);
        public delegate void EnemyHit(Hit hit);
        public delegate void IllegalHit(Hit hit);

        

        private event DataReceived DataReceivedEvent;
        private event FirstTimeDataReceived FirstTimeDataReceivedEvent;

        private delegate void DataReceived(NetworkMessage data);
        private delegate void FirstTimeDataReceived(string data);

        private bool listeningForIncommingRequests;
        
        private Socket listener;
        private Encoding encoding = Encoding.Unicode;
        private Stack<NetworkMessage> SendQueue;

        public NetworkCommunicator()
        {
            listeningForIncommingRequests = false;

            FirstTimeDataReceivedEvent += NetworkCommunicator_FirstTimeDataReceivedEvent;
            DataReceivedEvent += NetworkCommunicator_DataReceivedEvent;
        }

        public async Task Connect(IPEndPoint endpoint, ProtocolType protocol)
        {
            RemotePlayer = new NetworkPlayer()
            {
                Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, protocol),
                Buffer = new ArraySegment<byte>(new byte[8192]),
                ReceiveTask = Listen(),
                HeartbeatTask = Heartbeat()
            };

            await RemotePlayer.Socket.ConnectAsync(endpoint);
            RemotePlayer.ReceiveTask = Listen();
            RemotePlayer.ReceiveTask.Start();
        }

        private void NetworkCommunicator_FirstTimeDataReceivedEvent(string data)
        {
            NetworkMessage message = JsonConvert.DeserializeObject<NetworkMessage>(data);

            if ((string)message.Value == JoinPassword) {
                RemotePlayer.Authentified = true;
                
                PlayerJoinedEvent();
            }
        }

        private void NetworkCommunicator_DataReceivedEvent(NetworkMessage data)
        {
            switch (data.Type) {
                case NetworkMessage.MessageType.Connect:
                    break;
                case NetworkMessage.MessageType.Disconnect:
                    break;
                case NetworkMessage.MessageType.Heartbeat:
                    break;
                case NetworkMessage.MessageType.GameReady:
                    break;
                case NetworkMessage.MessageType.GameEnded:
                    break;
                case NetworkMessage.MessageType.PlayerReady:
                    break;
                case NetworkMessage.MessageType.ChatMessage:
                    break;
                case NetworkMessage.MessageType.PlayerHit:
                    break;
                case NetworkMessage.MessageType.EnemyHit:
                    break;
                case NetworkMessage.MessageType.IllegalHit:
                    break;
                default:
                    break;
            }
        }

        public async Task CreateServer(int port, ProtocolType protocolType, bool useUPnP)
        {
            if (useUPnP)
                await NatRegisterPort(port);

            IPEndPoint local = new IPEndPoint(IPAddress.Any, port);

            listener = new Socket(local.AddressFamily, SocketType.Stream, protocolType);
            listener.Listen(1);

            listeningForIncommingRequests = true;

            await Listen();
        }

        public void SendMessage(NetworkMessage message)
        {
            SendQueue.Push(message);
        }

        public void SendChatMessage(string content)
        {

        }

        public void SetReady()
        {

        }

        public void Hit(Hit hit)
        {

        }

        private async Task Listen()
        {
            while (listeningForIncommingRequests) {
                Socket remote = await listener.AcceptAsync();

                RemotePlayer = new NetworkPlayer(remote);

                await Receive();
            }
        }

        private async Task Heartbeat()
        {
            while (RemotePlayer.Socket.Connected) {
                await SendAsync(new NetworkMessage(NetworkMessage.MessageType.Heartbeat, DateTime.Now));

                await Task.Delay(1000);
            }
        }

        private async Task Receive()
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>();
            bool receiving = true;

            while (receiving) {
                string data = string.Empty;

                int bytesReceived = await RemotePlayer.Socket.ReceiveAsync(buffer, SocketFlags.None);

                data += encoding.GetString(buffer.Array, 0, bytesReceived);

                if (data.IndexOf("<EOF>") > -1)
                    break;

                data = data.TrimEnd("<EOF>".ToCharArray());

                NetworkMessage message = JsonConvert.DeserializeObject<NetworkMessage>(data);

                if (RemotePlayer.Authentified) {
                    DataReceivedEvent(message);
                } else {
                    FirstTimeDataReceivedEvent(data);
                }
            }
        }

        public async Task SendAsync(NetworkMessage message)
        {
            SendQueue.Push(message);
            await ClearSendQueue();
        }

        public void Send(NetworkMessage message)
        {
            SendQueue.Push(message);
            ClearSendQueue().Start();
        }

        private async Task Send(byte[] data)
        {
            await RemotePlayer.Socket.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
        }

        private async Task ClearSendQueue()
        {
            while (SendQueue.Count != 0) {
                NetworkMessage message = SendQueue.Pop();

                string json = JsonConvert.SerializeObject(message) + "<EOF>";
                byte[] data = encoding.GetBytes(json);

                await Send(data);
            }
        }

        public async Task NatRegisterPort(int port)
        {
            NatDiscoverer discoverer = new NatDiscoverer();
            NatDevice natDevice = await discoverer.DiscoverDeviceAsync();

            Mapping mapping = new Mapping(Protocol.Tcp, port, port, int.MaxValue, "NavalBattle"); //int.MaxValue: Auto-renew
            await natDevice.CreatePortMapAsync(mapping);
        }
    }

    /// <summary>
    /// Represents a player on the internet.
    /// </summary>
    public class NetworkPlayer
    {
        public const int BUFFER_SIZE = 8192;

        public ArraySegment<byte> Buffer;
        public Socket Socket;
        public Task ReceiveTask;
        public Task HeartbeatTask;

        public bool Authentified;
        public SimpleUserDataModel PlayerData;

        public NetworkPlayer()
        {
            Authentified = false;
            Buffer = new ArraySegment<byte>(new byte[BUFFER_SIZE]);
        }

        public NetworkPlayer(Socket socket)
        {
            Buffer = new ArraySegment<byte>(new byte[BUFFER_SIZE]);
            Socket = socket;
            ReceiveTask = null;
            HeartbeatTask = null;
            Authentified = false;
            PlayerData = null;
        }
    }

    /// <summary>
    /// Data sent to another player.
    /// </summary>
    public struct NetworkMessage
    {
        public MessageType Type;
        public object Value;

        public NetworkMessage(MessageType type)
        {
            Type = type;
            Value = null;
        }

        public NetworkMessage(MessageType type, object value)
        {
            Type = type;
            Value = value;
        }

        public enum MessageType
        {
            Connect,
            Disconnect,
            Heartbeat,

            GameReady,
            GameEnded,
            PlayerReady,
            ChatMessage,
            PlayerHit,
            EnemyHit,
            IllegalHit,
        }
    }
}
