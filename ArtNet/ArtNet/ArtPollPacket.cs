using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNet {

    namespace ArtPacket {

        public class ArtPollPacket : Packet {

            private static readonly int packet_length = 14;

            protected static readonly int TALKTOME_START_BYTE = 12;
            protected static readonly int PRIORITY_START_BYTE = 13;

            public ArtPollPacket() : base(packet_length) {
                //data = Const.GetZeroByteArray(packet_length);
                OpCode = Const.OPCODE_OpPoll;
            }

            public ArtPollPacket(byte[] dat) : base(packet_length) {
                if ((dat.Length == packet_length) && 
                    (Const.OPCODE_OpPoll == (int)dat[OPCODE_START_BYTE])) {
                    data = dat;
                }
            }

            public bool[] TalkToMeSettings {
                set {
                    if (value.Length == 8) {
                        byte[] set = new byte[1];
                        var bitArray = new BitArray(set);
                        for (int i = 0; i < value.Length; i++) {
                            if (value[i] == true) {
                                bitArray.Set(i, true);
                            }
                            else {
                                bitArray.Set(i, false);
                            }
                        }
                        bitArray.CopyTo(set, 0);
                        data[TALKTOME_START_BYTE] = set[0];
                    }
                }
                get {
                    bool[] set = new bool[8];
                    for (int i = 0; i < set.Length; i++) {
                        set[i] = Const.GetBitValue(data[TALKTOME_START_BYTE], i);
                    }
                    return set;
                }
            }

            public int Priority {
                set {
                    data[PRIORITY_START_BYTE] = (byte)value;
                }
                get {
                    return (int)data[PRIORITY_START_BYTE];
                }
            }
        }
    }
}
