using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ArtNet.ArtPacket;

namespace ArtNet {
    public class ArtNetReader {

        private UdpClient udpListener;
        private IPEndPoint ipEndpoint;
        private Thread readPacketThread;
        private ArtDispatcher artDispatcher;

        public ArtNetReader(){
            init(null);
        }

        public ArtNetReader(ArtDispatcher dispatcher) {
            init(dispatcher);
        }

        private void init(ArtDispatcher dispatcher) {


            //udpListener = new UdpClient(Const.BIND_PORT);
            //ipEndpoint = new IPEndPoint(IPAddress.Any, Const.BIND_PORT);
            ipEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), Const.BIND_PORT);
            udpListener = new UdpClient();
            udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpListener.Client.Bind(ipEndpoint);

            
            artDispatcher = dispatcher;
        }
        /// <summary>
        /// Starts packet recieving and dispatching in a new thread
        /// </summary>
        public void Start() {
            readPacketThread = new Thread(new ThreadStart(this.readPacket));
            readPacketThread.Start();
        }
        /// <summary>
        /// Stops packet recieving and dispatching thread
        /// </summary>
        public void Stop() {
            readPacketThread.Abort();
            readPacketThread.Join();
        }

        private void readPacket() {
            while (true) {
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
        }
    }
}
