using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtNet;

namespace ArtNet
{
    namespace ArtPacket {
        public abstract class Packet {

            protected readonly string name = Const.ARTNET_STR;
            internal static readonly int OPCODE_START_BYTE = 8;
            protected static readonly int VERSION_START_BYTE = 10;
            protected static readonly string NULL_DATA_STRING =
                "Packet shouts: your fucking data is null, chek it check it!";

            protected byte[] data;

            protected Packet(int length) {
                data = Const.GetZeroByteArray(length);
                setArtNetName();
                setProtoclVersion(Const.PROTOCOL_VERSION);
            }

            public virtual byte[] Data {
                get {
                    if (data == null) {
                        throw new NullReferenceException(
                            "Packet says: theres no fucking data yet or i have lost it somehow!");
                    }
                    return this.data;
                }

                set {
                    if (value == null) {
                        throw new NullReferenceException(NULL_DATA_STRING);
                    }
                    this.data = value;
                }
            }

            public virtual int OpCode {
                get {
                    if (data == null) {
                        throw new NullReferenceException(
                            "Packet says: my data is doomd, i dont know what the hell i am.");
                    }
                    return Const.LowByteHiByteToInt(
                        data[OPCODE_START_BYTE], 
                        data[OPCODE_START_BYTE + 1]
                        );
                }
                
                // this should be checked wether byte order is corect
                set {
                    if (data == null) {
                        throw new ArgumentNullException(NULL_DATA_STRING);
                    }
                    Const.PasteInt16ToByteArray((short)value, this.data, OPCODE_START_BYTE);
                }
            }

            public virtual int Version {
                get {
                    if (data == null) {
                        throw new ArgumentNullException(NULL_DATA_STRING);
                    }
                    return Const.BytesToInt(data, VERSION_START_BYTE, 2); // 2 is version length in bytes 
                                                                          // see ArtNet protocol doc.
                }
            }

            public int Length {
                get{
                    return data.Length;
                }
            }

            private void setProtoclVersion(int pv) {
                if (data == null) {
                    throw new ArgumentNullException(NULL_DATA_STRING);
                }
                byte[] b = BitConverter.GetBytes((short)pv);
                var index = 1;
                for (int i = 0; i < b.Length; i++) {
                    data[VERSION_START_BYTE + index] = b[i];
                    index--;
                }
            }

            private void setArtNetName() {
                if (data == null) {
                    throw new ArgumentNullException(NULL_DATA_STRING);
                }
                byte[] byteStr = ASCIIEncoding.ASCII.GetBytes(Const.ARTNET_STR);
                for (int i = 0; i < byteStr.Length; i++) {
                    data[i] = byteStr[i];
                }
            }
        }
    }
}
