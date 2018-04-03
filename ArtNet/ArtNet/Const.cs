using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ArtNet {
    public static class Const {

        public static readonly string ARTNET_STR                = "Art-Net";

        public static readonly int BIND_PORT                    = 0x1936;

        public static readonly string DEFAULT_BROADCAST_ADDRESS = "192.168.0.255";
	    public static readonly int FIRMWARE_VERSION             = 0;
        public static readonly int PROTOCOL_VERSION             = 14;
        public static readonly int OEM_CODE                     = 2048;
        public static readonly int UBEA_VERSION                 = 0;


        public static readonly int OPCODE_OpPoll                = 0x2000;
        public static readonly int OPCODE_OpPollReply           = 0x2100;
        public static readonly int OPCODE_OpDmx                 = 0x5000;
        public static readonly int OPCODE_OpSync                = 0x5200;

        public static int LowByteHiByteToInt(byte first, byte second) {
            byte[] b = { first, second };
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(b);
            }
            return BitConverter.ToInt16(b, 0);
        }

        public static int HiByteLowByteToInt(byte first, byte second) {
            byte[] b = new byte[2];
            b[0] = second;
            b[1] = first;
            return BitConverter.ToInt16(b, 0);
        }

        public static int BytesToInt(byte[] d, int startByte, int length) {
            byte[] b = new byte[length];
            for (int i = 0; i < length; i++) {
                b[i] = d[startByte + i];
            }
            return BitConverter.ToInt32(b, 0);
        }

        // length is optional 2 or 4.. short or int..
        public static void PasteInt16ToByteArray(short val, byte[] target, int startIndex) {
            PasteInt16ToByteArray(val, target, startIndex, PasteMode.LOW_HIGH);
        }

        public enum PasteMode {
            HIGH_LOW,
            LOW_HIGH
        }
        public static void PasteInt16ToByteArray(short val, byte[] target, int startIndex, PasteMode mode) {
            byte[] b = BitConverter.GetBytes(val);
            if (mode == PasteMode.LOW_HIGH) {
                for (int i = 0; i < b.Length; i++) {
                    target[startIndex + i] = b[i];
                }
            } else {
                int targetIndex = b.Length - 1;
                for (int i = 0; i < b.Length; i++) {
                    target[startIndex + targetIndex] = b[i];
                    targetIndex--;
                }
            }
        }

        public static void PasteByteArrayToByteArray(byte[] ba, byte[] target, int startIndex) {
            if (ba.Length + startIndex > target.Length) {
                throw new ArgumentOutOfRangeException("byte array too large for copying in target array.");
            }
            for (int i = 0; i < ba.Length; i++) {
                target[i + startIndex] = ba[i];
            }
        }

        public static byte[] GetZeroByteArray(int length) {
            byte[] b = new byte[length];
            for (int i = 0; i < length; i++) {
                b[i] = (byte)0;
            }
            return b;
        }

        public static string GetIpString(byte[] dat, int index) {
            StringBuilder s = new StringBuilder();
            for (int i = index; i < index + 4; i++) {
                if ((int)dat[i] < 0) {
                    s.Append(((int)dat[i] + 256) + "."); // if negative -> convert to unisgned
                }
                else {
                    s.Append((int)dat[i] + ".");
                }
            }
            return s.ToString().Substring(0, s.Length - 1);
        }

        public static bool GetBitValue(byte b,int index) {
            BitArray bitarray = new BitArray(b);
            return bitarray.Get(index);
        }

        public static int FindIndexOfValueInByteArray(int value, byte[] data, int offset, int length) {
            for (int i = 0; i < length; i++) {
                if (data[offset + i] == 0) {
                    return i;
                }
            }
            return -1; //if value not found.
        }

        public static string GetASCIIFromByteArray(byte[] data, int index, int length) {
            byte[] b = new byte[length];
            for (int i = 0; i < b.Length; i++) {
                b[i] = data[index + i];
            }
            return System.Text.Encoding.ASCII.GetString(b);
        }

        public static int ToUnsignedInt32Value(byte b) {
            var val = (int)b;
            if (val < 0) val += 256;
            return val;
        }

        public static string GetLocalhost() {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList) {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            throw new Exception("Loacl Ip address not found.");
        }

        public static bool validateIpv4(string ip) {
            if(String.IsNullOrWhiteSpace(ip)) {
                return false;
            }

            string[] splitValues = ip.Split('.');
            if (splitValues.Length != 4) {
                return false;
            }

            byte tempForParse;

            return splitValues.All(r => byte.TryParse(r, out tempForParse));
        }

        public static void PasteASCIIToByteArray(string str, byte[] target, int startByte, int maxLength) {
            var byteString = System.Text.Encoding.ASCII.GetBytes(str);
            if (byteString.Length > maxLength) {
                throw new ArgumentOutOfRangeException("string length is out of range -> value not set.");
            }
            for (int i = 0; i < byteString.Length; i++) {
                target[startByte + i] = byteString[i];
            }
        }

        public static byte[] GetMacAddressOfLocalMachine() {
            NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();
            string macAddr = string.Empty;
            foreach (NetworkInterface adapter in ni) {
                if (macAddr == string.Empty) {
                    macAddr = adapter.GetPhysicalAddress().ToString();
                }
            }
            var macAdress = new byte[macAddr.Length / 2];
            var index = 0;
            for (int i = 0; i < macAddr.Length; i += 2) {
                var subStr = macAddr.Substring(i, 2);
                var val = Convert.ToInt32(subStr, 16);
                macAdress[index] = (byte)val;
                index++;
            }
            return macAdress;
        }

    }
}
