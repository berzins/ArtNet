using System;
using System.Linq;
using ArtNet.ArtPacket;
using ArtNet;
using Microsoft.Kinect;


namespace ConsoleApplication1 {
    class Program {

        static void Main(string[] args) {

            Packet artPollPacket = new ArtPollPacket();
            ArtDmxPacket artDmxPacket = new ArtDmxPacket();
            ArtPollReplyPacket artPollReplyPacket = new ArtPollReplyPacket();
            byte[] dmxData = Enumerable.Repeat((byte)0, 512).ToArray();
            ArtNetWritter artNetOut = new ArtNetWritter();
            ArtDispatcher dispatcher = new ArtDispatcher();
            dispatcher.AddArtDmxListener(new ArtDmxListener());
            dispatcher.AddArtPollListener(new ArtPollListener());
            dispatcher.AddArtPollReplyListener(new ArtPollReplyListener());

            ArtNetReader artNetIn = new ArtNetReader(dispatcher);
            artNetIn.Start();

            KinectSensor sensor = KinectSensor.GetDefault();
            sensor.Open();
            InfraredFrameReader reader = sensor.InfraredFrameSource.OpenReader();
            FrameDescription fd = sensor.InfraredFrameSource.FrameDescription;

            
            ArtDmxPacket[] dmx = new ArtDmxPacket[2];
            for (int i = 0; i < dmx.Length; i++) {
                dmx[i] = new ArtDmxPacket();
                dmx[i].Universe = i;
                dmx[i].DmxData = dmxData;
            }

            while (true) {
                string str = Console.ReadLine();
                var value = Int32.Parse(str);
                if (value >= 0 && value <= 255) {
                    dmxData = Enumerable.Repeat((byte)value, 512).ToArray();
                }
                artDmxPacket.DmxData = dmxData;
                artNetOut.SetPacket(new ArtDmxPacket());
                artNetOut.Write();

            }
        }

        public void run() {

        }

        private void InfraredFrameArrived(InfraredFrameReader sender,
                                          InfraredFrameArrivedEventArgs args) {
            using (InfraredFrame frame = args.FrameReference.AcquireFrame()) {
                if (frame != null) {
                    
                }
            }

        }


    }

    public class ArtDmxListener : ArtListener {
        void ArtListener.Action(Packet p) {
            Console.WriteLine("yea, i got dmx! :)");
        }
    }

    public class ArtPollListener : ArtListener {
        void ArtListener.Action(Packet p) {
            Console.WriteLine("i got poll packet");
        }
    }

    public class ArtPollReplyListener : ArtListener {
        void ArtListener.Action(Packet p) {
            Console.WriteLine("i got artPollreply packet");
        }
    }
}
