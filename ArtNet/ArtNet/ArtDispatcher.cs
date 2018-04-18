using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using ArtNet.ArtPacket;

namespace ArtNet {
    public class ArtDispatcher {

        private List<ArtListener> artDmxListeners;
        private List<ArtListener> artPollListeners;
        private List<ArtListener> artPollReplyListeners;
        private Queue<byte[]> packetQueue;


        public ArtDispatcher() {
            packetQueue = new Queue<byte[]>();
            artDmxListeners = new List<ArtListener>();
            artPollListeners = new List<ArtListener>();
            artPollReplyListeners = new List<ArtListener>();
        }

        internal void Dispatch(byte[] packet) {
           // packetQueue.Enqueue(packet);
            int opCode = this.getOpCode(packet);
            //Console.WriteLine("opCode = " + opCode);

            if (opCode == Const.OPCODE_OpDmx) {
                Utils.ArtPacketStopwatch = System.Diagnostics.Stopwatch.StartNew(); 
                notifyListeners(artDmxListeners, new ArtDmxPacket(packet));
            }
            else if (opCode == Const.OPCODE_OpPoll) {
                notifyListeners(artPollListeners, new ArtPollPacket(packet));
            }
            else if (opCode == Const.OPCODE_OpPollReply) {
                notifyListeners(artPollReplyListeners, new ArtPollReplyPacket(packet));
            }

        }

        public void AddArtDmxListener(ArtListener listener) {
            addListener(listener, artDmxListeners);
        }

        public void AddArtPollReplyListener(ArtListener listener) {
            addListener(listener, artPollReplyListeners);
        }

        public void AddArtPollListener(ArtListener listener) {
            addListener(listener, artPollListeners);
        }

        //-------------------PRIVATE METHODS-----------------------

        private int getOpCode(byte[] packet) {
            return Const.LowByteHiByteToInt(
                packet[Packet.OPCODE_START_BYTE + 1], packet[Packet.OPCODE_START_BYTE]);
        }

        private void addListener(ArtListener listener, List<ArtListener> observers) {
            checkForNull(listener);
            observers.Add(listener);
        }

        private void checkForNull(object o) {
            if (o == null) {
                throw new ArgumentNullException();
            }
        }

        private void notifyListeners(List<ArtListener> observers, Packet packet) {
            foreach (ArtListener al in observers) {
                al.Action(packet);
            }
        }
    }
}
