using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ArtNet.ArtPacket;
using System.Net.NetworkInformation;

namespace ArtNet {
    public class ArtNetReader {

        private UdpClient udpListener;
        private IPEndPoint ipEndpoint;
        private Thread readPacketThread;
        private ArtDispatcher artDispatcher;
        private bool running = false;

        public ArtNetReader(){
            init(null, IPAddress.Parse("127.0.0.1"));
        }

        public ArtNetReader(ArtDispatcher dispatcher) {
            init(dispatcher, IPAddress.Parse("127.0.0.1"));
        }

        public ArtNetReader(ArtDispatcher dispatcher, IPAddress bindIp)
        {
            init(dispatcher, bindIp);
        }

        private void init(ArtDispatcher dispatcher, IPAddress bindIp) {


            //udpListener = new UdpClient(Const.BIND_PORT);
            //ipEndpoint = new IPEndPoint(IPAddress.Any, Const.BIND_PORT);
            //ipEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), Const.BIND_PORT);
            ipEndpoint = new IPEndPoint(bindIp, Const.BIND_PORT);
            artDispatcher = dispatcher;
        }

        private void initUdp() {
            try
            {
                udpListener = new UdpClient();
                udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpListener.Client.Bind(ipEndpoint);
                Logger.Log("Art Net Reader binded to " + ipEndpoint.Address + ":" + ipEndpoint.Port, LogLevel.INFO);
            }
            catch (Exception e) {
                Logger.Log("ArtNet reader failed to bin ip " + ipEndpoint.Address + " : "
                    + e.Message + " : " + e.StackTrace, LogLevel.ERROR);
            }
        }

        /// <summary>
        /// Starts packet recieving and dispatching in a new thread
        /// </summary>
        public void Start() {
            running = true;
            readPacketThread = new Thread(new ThreadStart(this.readPacket));
            readPacketThread.Start();
            Logger.Log("Art Net Reader started", LogLevel.INFO);
        }

        /// <summary>
        /// Exits art packet reading loop
        /// </summary>
        public void Stop() {

            // set listening loop to false
            running = false; 

            //give it a last punch in case noone is sending enything
            var soc = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Const.BIND_PORT);
            soc.SendTo(Encoding.ASCII.GetBytes("go home"), endp);
        }

        /// <summary>
        /// Sets udp listener ip address
        /// Incase upd Listener is listening. It stops, sets and starts again it
        /// </summary>
        public void SetBindIp(IPAddress bindIp) {
            // in case that happens while running
            if (udpListener != null) {
                Stop();
                ipEndpoint = new IPEndPoint(bindIp, Const.BIND_PORT);
                Start();
            }
            ipEndpoint = new IPEndPoint(bindIp, Const.BIND_PORT);
        }

        private void readPacket() {
            if (udpListener == null) {
                initUdp();
            }
            while (running) {
                var packet = udpListener.Receive(ref ipEndpoint);

                var s = Const.GetASCIIFromByteArray(
                    packet, 0, Const.ARTNET_STR.Length
                    );
                if (artDispatcher == null) {
                    throw new ArgumentNullException("Dispatcher is null");
                }
                if (s == Const.ARTNET_STR) {
                    artDispatcher.Dispatch(packet);
                }
            }
            udpListener.Close();
            udpListener = null;
            Logger.Log("ArtNet reader stopped", LogLevel.INFO);
        }

        public List<IPAddress> GetAvailableBindAddresses() {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach(NetworkInterface ni in interfaces)
            {

                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses) {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    }
                }
            }
            return null;
        }
    }
}
