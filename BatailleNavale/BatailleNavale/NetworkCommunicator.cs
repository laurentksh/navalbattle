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

        public bool Connected
        {
            get {
                if (!RemotePlayer.Socket.Connected)
                    return false;
                if (Ping >= 3000)
                    return false;

                return true;
            }
            private set { }
        }
        public int Ping { get; private set; }
        public string JoinPassword { get; set; }
        public NetworkPlayer RemotePlayer { get; set; }


        public event PlayerJoinedDelegate PlayerJoinedEvent;
        public event PlayerLeftDelegate PlayerLeftEvent;
        public event PlayerReadyDelegate PlayerReadyEvent;
        public event ConnectedDelegate ConnectedEvent;
        public event GameReadyDelegate GameReadyEvent;
        public event GameEndedDelegate GameEndedEvent;
        public event ChatMessageReceivedDelegate ChatMessageReceivedEvent;
        public event EnemyHitDelegate EnemyHitEvent;
        public event IllegalHitDelegate IllegalHitEvent;

        public delegate void PlayerJoinedDelegate();
        public delegate void PlayerLeftDelegate();
        public delegate void PlayerReadyDelegate(List<BoatModel> boats);
        public delegate void GameReadyDelegate();
        public delegate void ConnectedDelegate();
        public delegate void GameEndedDelegate(MessagesData.GameEndedData data);
        public delegate void ChatMessageReceivedDelegate(string content);
        public delegate void EnemyHitDelegate(Hit hit);
        public delegate void IllegalHitDelegate(Hit hit);

        

        private event DataReceived DataReceivedEvent;
        private event FirstTimeDataReceived FirstTimeDataReceivedEvent;

        private delegate void DataReceived(NetworkMessage data);
        private delegate void FirstTimeDataReceived(string data);

        private bool listeningForIncommingRequests;
        private DateTime LastHeartbeat;

        private Socket listener;
        private Encoding encoding = Encoding.Unicode;
        private Stack<NetworkMessage> SendQueue;

        public NetworkCommunicator()
        {
            listeningForIncommingRequests = false;

            SendQueue = new Stack<NetworkMessage>();

            FirstTimeDataReceivedEvent += NetworkCommunicator_FirstTimeDataReceivedEvent;
            DataReceivedEvent += NetworkCommunicator_DataReceivedEvent;
        }

        public async Task Connect(IPEndPoint endpoint, ProtocolType protocol, SimpleUserDataModel userData)
        {
            RemotePlayer = new NetworkPlayer()
            {
                Socket = new Socket(endpoint.AddressFamily, SocketType.Stream, protocol),
                Buffer = new ArraySegment<byte>(new byte[8192]),
                ReceiveTask = Listen(),
                HeartbeatTask = Heartbeat()
            };

            await RemotePlayer.Socket.ConnectAsync(endpoint);
            RemotePlayer.ReceiveTask = Task.Run(Receive);

            NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.Connect, userData);
            await SendAsync(message);
        }

        private void NetworkCommunicator_FirstTimeDataReceivedEvent(string data)
        {
            NetworkMessage message = JsonConvert.DeserializeObject<NetworkMessage>(data);

            RemotePlayer.Authentified = true;

            RemotePlayer.PlayerData = JsonConvert.DeserializeObject<SimpleUserDataModel>(message.Value);

            PlayerJoinedEvent();

            NetworkMessage message2 = new NetworkMessage(NetworkMessage.MessageType.ConnectAccepted);
            Send(message2);

            /*if (JsonConvert.DeserializeObject<string>(message.Value) == JoinPassword) {
                RemotePlayer.Authentified = true;
                
                PlayerJoinedEvent();
            }*/
        }

        private void NetworkCommunicator_DataReceivedEvent(NetworkMessage data)
        {
            object value = JsonConvert.DeserializeObject(data.Value);
            Console.WriteLine("abc");
            switch (data.Type) {
                case NetworkMessage.MessageType.Connect:
                    
                    break;
                case NetworkMessage.MessageType.Disconnect:
                    PlayerLeftEvent();
                    break;
                case NetworkMessage.MessageType.Heartbeat:
                    Ping = (DateTime.Now - LastHeartbeat).Milliseconds;
                    break;
                case NetworkMessage.MessageType.GameReady:
                    break;
                case NetworkMessage.MessageType.GameEnded:
                    MessagesData.GameEndedData gameEnded = (MessagesData.GameEndedData)value;

                    GameEndedEvent(gameEnded);
                    break;
                case NetworkMessage.MessageType.PlayerReady:
                    PlayerReadyEvent((List<BoatModel>)value);
                    break;
                case NetworkMessage.MessageType.ChatMessage:
                    ChatMessageReceivedEvent(JsonConvert.DeserializeObject<string>(data.Value));
                    break;
                case NetworkMessage.MessageType.PlayerHit:
                    EnemyHitEvent((Hit)value);
                    break;
                case NetworkMessage.MessageType.IllegalHit:
                    IllegalHitEvent((Hit)value);
                    break;
                case NetworkMessage.MessageType.ConnectAccepted:
                    ConnectedEvent();
                    break;
                default:
                    break;
            }
        }

        public async Task CreateServer(int port, ProtocolType protocolType, bool useUPnP)
        {
            IPEndPoint local = new IPEndPoint(IPAddress.Any, port);

            Console.WriteLine($"Hosting on {local} with protocol {protocolType}. UPNP: {useUPnP}");

            if (useUPnP) {
                try {
                    await NatRegisterPort(port);
                } catch (Exception) {
                    throw new NatDeviceNotFoundException();
                }
            }

            listener = new Socket(local.AddressFamily, SocketType.Stream, protocolType);
            listener.Bind(local);
            listener.Listen(1);

            listeningForIncommingRequests = true;

            await Listen().ConfigureAwait(false);
        }

        public void SendChatMessage(string content)
        {
            if (RemotePlayer != null && RemotePlayer.Socket.Connected) {
                NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.ChatMessage, content);
                Send(message);
            }
        }

        public void SetPlayerReady(List<BoatModel> boats)
        {
            NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.PlayerReady, boats);
        }

        public void Hit(Hit hit)
        {
            NetworkMessage message = new NetworkMessage(NetworkMessage.MessageType.PlayerHit, hit);
            Send(message);
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
                await SendAsync(new NetworkMessage(NetworkMessage.MessageType.Heartbeat, DateTime.Now)).ConfigureAwait(false);

                await Task.Delay(1000);
            }
        }

        private async Task Receive()
        {
            bool receiving = true;

            while (receiving) {
                string data = string.Empty;

                while (true) {
                    if (!RemotePlayer.Socket.Connected) {
                        receiving = false;
                        break;
                    }

                    int bytesReceived = await RemotePlayer.Socket.ReceiveAsync(RemotePlayer.Buffer, SocketFlags.None);

                    data += encoding.GetString(RemotePlayer.Buffer.Array, 0, bytesReceived);

                    if (data.IndexOf("<EOF>") > -1)
                        break;
                }

                Console.WriteLine(data);

                data = data.Remove(data.Length - 5, 5);

                NetworkMessage message = JsonConvert.DeserializeObject<NetworkMessage>(data);
                
                if (RemotePlayer.Authentified) {
                    await Task.Run(() =>
                    {
                        DataReceivedEvent(message);
                    }).ConfigureAwait(false);
                } else {
                    await Task.Run(() =>
                    {
                        FirstTimeDataReceivedEvent(data);
                    }).ConfigureAwait(false);
                }
            }
        }

        public async Task SendAsync(NetworkMessage message)
        {
            SendQueue.Push(message);
            await Task.Run(ClearSendQueue).ConfigureAwait(false);
        }

        public void Send(NetworkMessage message)
        {
            SendQueue.Push(message);
            Task.Run(ClearSendQueue).ConfigureAwait(false);
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

        public void Disconnect()
        {
            listeningForIncommingRequests = false;
            
            if (listener != null) {
                listener.Disconnect(false);
                listener.Dispose();
            }

            if (RemotePlayer != null) {
                RemotePlayer.Socket.Disconnect(false);
                RemotePlayer.Socket.Dispose();
            }
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
        public string Value;

        public NetworkMessage(MessageType type)
        {
            Type = type;
            Value = null;
        }

        public NetworkMessage(MessageType type, object value)
        {
            Type = type;
            Value = JsonConvert.SerializeObject(value);
        }

        public enum MessageType
        {
            /// <summary>Remote player connected / Send a connect request</summary>
            Connect,

            /// <summary>Connect request accepted.</summary>
            ConnectAccepted,
            /// <summary>Remote player disconnected / Send a disconnect request</summary>
            Disconnect,
            /// <summary>Heartbeat</summary>
            Heartbeat,

            /// <summary>Notify the remote player that the game is ready.</summary>
            GameReady,
            /// <summary>Notify the remote player that the game ended.</summary>
            GameEnded,
            /// <summary>Notify the host that we're ready.</summary>
            PlayerReady,
            /// <summary>Received a message / Send a chat message</summary>
            ChatMessage,
            /// <summary>Send a hit request.</summary>
            PlayerHit,
            /// <summary>A hit sent is invalid.</summary>
            IllegalHit,
        }
    }

    public class MessagesData
    {
        public struct GameEndedData
        {
            public GameResult Result;
            public List<BoatModel> EnemyBoats;
        }
    }
}
