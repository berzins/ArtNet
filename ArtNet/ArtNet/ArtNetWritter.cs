using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtNet.ArtPacket;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace ArtNet {
    public class ArtNetWritter {

        private Socket socket;
        private IPAddress broadcstIp;
        private IPEndPoint ipEndpoint;
        private Packet packet;

        public ArtNetWritter() {
            socket = null;
            broadcstIp = IPAddress.Parse("192.168.0.255");
            ipEndpoint = new IPEndPoint(broadcstIp, Const.BIND_PORT);
            packet = new ArtDmxPacket();
        }

        public void Write() {
            Write(packet);
        }

        public void Write(Packet p) {
            socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Dgram,
                    ProtocolType.Udp
                );
            try {
                socket.SendTo(p.Data, ipEndpoint);
            } catch (Exception e) {
                Console.WriteLine("oh no!" + e.Message + " ..... " + e.StackTrace);
            }
            socket.Close();
        }

        public void SetPacket(Packet p) {
            if (p != null) {
                packet = p;
            }
        }
    }
}
