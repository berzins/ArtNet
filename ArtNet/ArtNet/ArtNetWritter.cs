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
        private IPAddress ipAddress;
        private IPEndPoint ipEndpoint;
        private Packet packet;

        public ArtNetWritter() : this(IPAddress.Parse("255.255.255.255")) {}

        public ArtNetWritter(IPAddress ip) {
            socket = null;
            ipAddress = ip;
            ipEndpoint = new IPEndPoint(ipAddress, Const.BIND_PORT);
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
