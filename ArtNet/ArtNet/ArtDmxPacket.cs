using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNet {

    namespace ArtPacket {
       public class ArtDmxPacket : Packet {

            private static readonly int PACKET_LENGTH           = 512 + 19; // 512 is dmx data and 19 is packet header
            private static readonly int DEFAULT_DMX_DAT_LENGTH  = 512;
            private static readonly int SEQUENCE_START_BYTE     = 12;
            private static readonly int PHYSICAL_START_BYTE     = 13;
            private static readonly int UNIVERSE_START_BYTE     = 14;
            private static readonly int SUBNET_START_BYTE       = 15;
            private static readonly int DATA_LENGTH_START_BYTE  = 16;
            private static readonly int DMX_DATA_START_BYTE     = 18;

            public ArtDmxPacket() : base(PACKET_LENGTH) {
                this.intitToDefault();
            }

            public ArtDmxPacket(int subNet, int universe) : base(PACKET_LENGTH) {
                this.intitToDefault();
                SubNet = subNet;
                Universe = universe;
            }

            public ArtDmxPacket(byte[] data) : base(PACKET_LENGTH) {
                if (data != null) {
                    this.data = data;
                }
            }

            //-------------PROPERTIES---------------
            public int SequenceIndex {
                set {
                    this.data[SEQUENCE_START_BYTE] = (byte)value;
                }
                get {
                    return Convert.ToInt32(this.data[SEQUENCE_START_BYTE]);
                }
            }

            public int PhysicalPort {
                set {
                    this.data[PHYSICAL_START_BYTE] = (byte)value;
                }
                get {
                    return Convert.ToInt32(this.data[PHYSICAL_START_BYTE]);
                }
            }

            public int Universe {
                set {
                    this.data[UNIVERSE_START_BYTE] = (byte)value;
                }
                get {
                    return Convert.ToInt32(this.data[UNIVERSE_START_BYTE]);
                }
            }

            public int SubNet {
                //WARNING!!!! i'm not sure it is set correct but i dont have anything to test against.
                set {
                    this.data[SUBNET_START_BYTE] = (byte)value;
                }
                // --- END OF WARNING;
                get {
                    return Convert.ToInt32(this.data[SUBNET_START_BYTE]);
                }
            }

            public int DmxDataLength {
                set {
                    if (value >= 0 && value <= 512) {
                        Const.PasteInt16ToByteArray(
                            (short)value, this.data, DATA_LENGTH_START_BYTE, Const.PasteMode.HIGH_LOW);
                    }
                    else {
                        throw new ArgumentOutOfRangeException();
                    }
                }
                get {
                    return Const.LowByteHiByteToInt(
                        data[DATA_LENGTH_START_BYTE], data[DATA_LENGTH_START_BYTE + 1]);
                }
            }

            public byte[] DmxData {
                set {
                    if (value.Length >= 0 && value.Length <= 512) {
                        for (int i = 0; i < value.Length; i++) {
                            data[DMX_DATA_START_BYTE + i] = value[i];
                        }
                    }
                }
                get {
                    byte[] b = new byte[DmxDataLength];
                    for (int i = 0; i < b.Length; i++) {
                        b[i] = data[DMX_DATA_START_BYTE + i];
                    }
                    return b;
                }
            }

            //------------METHODS------------
            public void SetDmxChanelValue(int chanel, int value) {
                if((chanel >= 0 && chanel < 512) && (value >= 0 && value < 256)) {
                    data[DMX_DATA_START_BYTE + chanel] = (byte)value;
                }
            }

            public int GetDmxChannelValue(int chanel) {
                if (chanel >= 0 && chanel < 512) {
                    return Convert.ToInt32(data[DMX_DATA_START_BYTE + chanel]);
                }
                return -1; //error value if channel is out of range
            }

            // sets all dmx chanels to maximal value
            public void FullOn() {
                byte max = (byte)255;
                for (int i = 0; i < DEFAULT_DMX_DAT_LENGTH; i++) {
                    data[DMX_DATA_START_BYTE + i] = max;
                }
            }

            //sets all dmx cahnels to minimal value
            public void Blackout() {
                byte min = (byte)0;
                for (int i = 0; i < DEFAULT_DMX_DAT_LENGTH; i++) {
                    data[DMX_DATA_START_BYTE + i] = min;
                }
            }

            private void intitToDefault() {
                //data = new byte[PACKET_LENGTH];
                OpCode = Const.OPCODE_OpDmx;
                SequenceIndex = 0;
                PhysicalPort = 0;
                Universe = 0;
                SubNet = 0;
                DmxDataLength = DEFAULT_DMX_DAT_LENGTH;
                Blackout();
            }
        }
    }
}
