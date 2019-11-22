using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using BatailleNavale.Model;
using Open.Nat;

namespace BatailleNavale
{
    public class NetworkCommunication //Work-In-Progress
    {
        public event DataReceived DataReceivedEveht;

        public delegate void DataReceived(string data, NetworkPlayer netPlayer, EventArgs e);


        private Socket Listener;

        public NetworkCommunication()
        {
            
        }

        public void Connect(IPEndPoint endpoint)
        {
            
        }

        public void CreateServer(int port, ProtocolType protocolType)
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, protocolType);
            Listener.Listen(1);

            Task.Run(Listen);
        }

        private async Task Listen()
        {
            Socket remote = await Listener.AcceptAsync();

            //Task.Run(Receive);
        }

        private async Task Receive(Socket socket)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>();
            bool receiving = true;

            while (receiving) {
                await socket.ReceiveAsync(buffer, SocketFlags.None);
            }
        }

        public async Task NatRegisterPort(int port)
        {
            NatDiscoverer discoverer = new NatDiscoverer();
            NatDevice natDevice = await discoverer.DiscoverDeviceAsync();

            Mapping mapping = new Mapping(Protocol.Tcp, port, port, int.MaxValue, "NavalBattle");
            await natDevice.CreatePortMapAsync(mapping);
        }
    }

    public struct NetworkPlayer
    {
        public ArraySegment<byte> Buffer;
        public Socket Socket;
        public Task ReceiveTask;

        public SimpleUserDataModel PlayerData;
    }
}
