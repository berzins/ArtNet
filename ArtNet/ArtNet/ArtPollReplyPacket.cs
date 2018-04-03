using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArtNet {
    namespace ArtPacket {
        public class ArtPollReplyPacket : Packet {
            private static readonly int packet_length               = 239;
            private static readonly int prot_type_ARTNET_in_out     = 133;
            private static readonly int good_input_DEFAULT          = 0;
            private static readonly int good_output_DEFAULT         = 0;
            private static readonly int min_port                    = 1;
            private static readonly int max_port                    = 4;
            private static readonly int mac_length                  = 6;
            private static readonly string mac_devider              = ":";
            private static readonly string ip_devider               = ".";
            private static readonly int ipv4_length                 = 4;
            private static readonly int ESTA_CODE                   = 0x4c4c;


            private static readonly int IP_START_BYTE               = 10; //ok
            private static readonly int ETHERNET_PORT_START_BYTE    = 14; //ok
            private static readonly int FIRMWARE_VERSION_START_BYTE = 16; //ok
            private static readonly int NETSWITCH_START_BYTE        = 18; //ok
            private static readonly int SUBSWITCH_START_BYTE        = 19; // ok
            private static readonly int OEM_CODE_START_BYTE         = 20; // ok
            private static readonly int UBEA_VERS_START_BYTE        = 22; // ok
            private static readonly int STATUS1_START_BYTE          = 23; //ok
            private static readonly int ESTA_CODE_START_BYTE        = 24; //ok
            private static readonly int SHORT_NAME_START_BYTE       = 26; // ok
            private static readonly int LONG_NAME_START_BYTE        = 44; //ok
            private static readonly int NODE_REPORT_START_BYTE      = 108; //ok
            private static readonly int PORT_COUNT_START_BYTE       = 172; //ok
            private static readonly int PORT_TYPE_START_BYTE        = 174; //ok
            private static readonly int GOOD_INPUT_START_BYTE       = 178; //ok
            private static readonly int GOOD_OUTPUT_START_BYTE      = 182; //ok
            private static readonly int SW_IN_START_BYTE            = 186; //ok
            private static readonly int SW_OUT_START_BYTE           = 190; //ok
            private static readonly int SW_VIDEO_START_BYTE         = 194; //ok
            private static readonly int SW_MACRO_START_BYTE         = 195; //ok
            private static readonly int SW_REMOTE_START_BYTE        = 196; //ok
            private static readonly int SPARE1_START_BYTE           = 197;
            private static readonly int SPARE2_START_BYTE           = 198;
            private static readonly int SPARE3_START_BYTE           = 199;
            private static readonly int STYLE_START_BYTE            = 200;
            private static readonly int MAC_ADDRESS_START_BYTE      = 201;
            private static readonly int BIND_IP_START_BYTE          = 207;
            private static readonly int BIND_INDEX_START_BYTE       = 211;
            private static readonly int STATUS2_START_BYTE          = 212;
            private static readonly int FILLER_START_BYTE           = 213;

            private static readonly int SHORT_NAME_LENGTH           = 18;
            private static readonly int LONG_NAME_LENGTH            = 64;
            private static readonly int NODE_REPORT_LENGTH          = 64;


            public static readonly int STATUS_OPTION_FRONT_PANEL     = 7;
            public static readonly int STATUS_OPTION_PORT_ADDRESS_PROG = 5;
            public static readonly int STATUS_OPTION_BOOT_MODE      = 2;
            public static readonly int STATUS_OPTION_RDM_CAPABLE    = 1;
            public static readonly int STATUS_OPTION_UBEA_SUPPORT   = 0;


            public ArtPollReplyPacket() : base(packet_length) {
                //data            = Const.GetZeroByteArray(packet_length);
                OpCode          = Const.OPCODE_OpPollReply;
                IP              = Const.GetLocalhost();
                EthernetPort    = Const.BIND_PORT;
                NetSwitch       = 0;
                SubSwitch       = 0;
                OemCode         = Const.OEM_CODE;
                UbeaVersion     = (Const.UBEA_VERSION);
                Status1         = new bool[] {false, false, false, false, false, false, false, false};
                ESTACodeValue   = ESTA_CODE;
                ShortName       = "Ze Sloka";
                LongName        = "Go go go go Zeeee Slokaaaaaaaaaaaa!";
                NodeReport      = "no such island";
                PortCount       = 4;
                SetDefaultPortType();
                SetDefaultSwIn();
                SetDefaultSwOut();
                Style           = 0;
                setSwVideo_SwMacro_SwRemote_to_zero();
                setSpareFieldsToZero();
                setMacAddress();
                BindIpString    = "0.0.0.0";
                BindIndex       = 0;
                Status2         = 0;
                setFiller();
            }

            public ArtPollReplyPacket(byte[] dat) : base (packet_length) {
                if (dat.Length != packet_length) {
                    throw new ArgumentOutOfRangeException("oh gosh");
                }
            }

            public string IP {
                get {
                    return Const.GetIpString(data, IP_START_BYTE);
                }
                set {
                    if (!Const.validateIpv4(value)) {
                        return;
                    }
                    byte[] ipBuff = IPAddress.Parse(value).GetAddressBytes();
                    for (int i = 0; i < ipBuff.Length; i++) {
                        data[IP_START_BYTE + i] = ipBuff[i];
                    }
                }

            }

            public int EthernetPort {
                get {
                    return Const.LowByteHiByteToInt(
                        data[ETHERNET_PORT_START_BYTE], 
                        data[ETHERNET_PORT_START_BYTE + 1]);
                }

                set {
                    Const.PasteInt16ToByteArray(
                        (short)value, data, ETHERNET_PORT_START_BYTE, Const.PasteMode.LOW_HIGH);
                }
            }

            public int FirmwareVersion {
                get {
                    return Const.LowByteHiByteToInt(
                        this.data[FIRMWARE_VERSION_START_BYTE], 
                        this.data[FIRMWARE_VERSION_START_BYTE + 1]);
                }
                set {
                    Const.PasteInt16ToByteArray(
                        (short)value, data, FIRMWARE_VERSION_START_BYTE, Const.PasteMode.HIGH_LOW);
                }
            }

            public int NetSwitch {
                get {
                    return Convert.ToInt32(data[NETSWITCH_START_BYTE]);
                }
                set {
                    data[NETSWITCH_START_BYTE] = (byte)value;
                }
            }

            public int SubSwitch {
                get {
                    return Convert.ToInt32(data[SUBSWITCH_START_BYTE]);
                }
                set {
                    data[SUBSWITCH_START_BYTE] = (byte)value;
                }
            }

            public int OemCode {
                get {
                    return Const.HiByteLowByteToInt(
                        data[OEM_CODE_START_BYTE], 
                        data[OEM_CODE_START_BYTE + 1]);
                }
                set {
                    Const.PasteInt16ToByteArray(
                        (short)value, data, OEM_CODE_START_BYTE, Const.PasteMode.LOW_HIGH);
                }
            }

            public int UbeaVersion {
                get {
                    return Convert.ToInt32(data[UBEA_VERS_START_BYTE]);
                }
                set {
                    data[UBEA_VERS_START_BYTE] = (byte)value;
                }
            }
            /// <summary>
            /// output: bool array with 8 elemetns what represent bit swithces...  
            /// input: bool array with 8 elemtns what represent bit switches
            /// </summary>
            public bool[] Status1 {
                get {
                    bool[] set = new bool[8]; // settings is 8 bits wide
                    for (int i = 0; i < set.Length; i++) {
                        set[i] = Const.GetBitValue(data[STATUS1_START_BYTE], i);
                    }
                    return set;
                }
                set {
                    if (value.Length != 8) {
                        throw new ArgumentOutOfRangeException("bool array should contain 8 elements");
                    }
                    byte b = (byte)0;
                    for (int i = 0; i < value.Length; i++) {
                        if (value[i] == true) {
                            b = (byte)((b << 1) | 1);
                        }
                        else {
                            b = (byte)((b << 1) | 0);
                        }
                    }
                }
            }

            public string Statuss1String {
                get {
                    StringBuilder sb = new StringBuilder();
                    var val = this.Status1;
                    for (int i = 0; i < val.Length; i++) {
                        sb.Append(val[i] ? "1" : "0");
                    }
                    return sb.ToString();
                }
            }

            public int ESTACodeValue {
                get {
                    return Const.LowByteHiByteToInt(
                        data[ESTA_CODE_START_BYTE], data[ESTA_CODE_START_BYTE + 1]);
                }
                set {
                    Const.PasteInt16ToByteArray(
                        (short)value, data, ESTA_CODE_START_BYTE, Const.PasteMode.LOW_HIGH);
                }
            }

            public string ShortName {
                get {
                    return extractString(SHORT_NAME_START_BYTE, 18); // short name is 18 bytes long
                }
                set {
                    Const.PasteASCIIToByteArray(
                        value, data, SHORT_NAME_START_BYTE, SHORT_NAME_LENGTH);
                }

            }

            public string LongName {
                get {
                    return extractString(LONG_NAME_START_BYTE, 64); // long name is 64 bytes long
                }
                set {
                    Const.PasteASCIIToByteArray(
                        value, data, LONG_NAME_START_BYTE, LONG_NAME_LENGTH);
                }

            }

            public string NodeReport {
                get {
                    return extractString(NODE_REPORT_START_BYTE, 64); // node report is 64 bytes long
                }
                set {
                    Const.PasteASCIIToByteArray(
                        value, data, NODE_REPORT_START_BYTE, NODE_REPORT_LENGTH);
                }

            }

            public int PortCount {
                // hi byte is not used in Art-Net version 14,
                // but reserverd for future expansion.
                // see Art-Net documentation for details
                get {
                    return (int)data[PORT_COUNT_START_BYTE + 1];
                }
                set {
                    data[PORT_COUNT_START_BYTE + 1] = (byte)value;
                }
            }

            public int Style {
                get {
                    return Const.ToUnsignedInt32Value(data[STYLE_START_BYTE]);
                }
                set {
                    if (value >= 0 && value < 256) {
                        byte b = (byte)value;
                        data[STYLE_START_BYTE] = b;
                    }
                    else {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public byte[] MacAddress {
                get {
                    return this.extractAddressByteArray(MAC_ADDRESS_START_BYTE, mac_length);
                }
                set {
                    if (value.Length == 6) {
                        Const.PasteByteArrayToByteArray(value, data, MAC_ADDRESS_START_BYTE);
                    }
                }
            }

            public string MacAddressString {
                get {
                    return extractAddressString(MacAddress, mac_devider);
                }
            }

            public byte[] BindIp {
                get {
                    return this.extractAddressByteArray(BIND_IP_START_BYTE, ipv4_length);
                }
                set {
                    if (value.Length <= ipv4_length) {
                        Const.PasteByteArrayToByteArray(value, data, BIND_IP_START_BYTE);
                    }
                    else {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public string BindIpString {
                get {
                    return extractAddressString(BindIp, ip_devider);
                }
                set {
                    if (!(value.IndexOf(".") > 0)) {
                        throw new FormatException("ip string siganture should be 'u8.u8.u8.u8'");
                    }

                    var splitStr = value.Split('.');
                    if (splitStr.Length != 4) {
                        throw new FormatException("ip string siganture should be 'u8.u8.u8.u8'");
                    }

                    byte[] b = new byte[splitStr.Length];
                    for (int i = 0; i < b.Length; i++) {
                        b[i] = (byte)Convert.ToInt16(splitStr[i]);
                    }
                }
            }

            public int BindIndex {
                get {
                    return Const.ToUnsignedInt32Value(data[BIND_INDEX_START_BYTE]);
                }
                set {
                    if (value >= 0 && value < 256) {
                        data[BIND_INDEX_START_BYTE] = (byte)value;
                    }
                }
            }

            public int Status2 {
                get {
                    return Const.ToUnsignedInt32Value(data[STATUS2_START_BYTE]);
                }
                set {
                    if (value >= 0 && value < 256) {
                        data[STATUS2_START_BYTE] = (byte)value;
                    }
                }
            }

            public string Status2String{
                get {
                    byte[] b = { data[STATUS2_START_BYTE] };
                    return System.Text.Encoding.ASCII.GetString(b);
                }
            }





           
            //------------------PUBLIC GETTERS-------------------

            public int GetProtType(int port) {
                return extractPortValue(PORT_TYPE_START_BYTE, port, min_port, max_port);
            }

            public int GetGoodInput(int port) {
                return extractPortValue(GOOD_INPUT_START_BYTE, port, min_port, max_port);
            }

            public int GetGoodOutput(int port) {
                return extractPortValue(GOOD_OUTPUT_START_BYTE, port, min_port, max_port);
            }

            public int GetSwIn(int port) {
                return extractPortValue(SW_IN_START_BYTE, port, min_port, max_port);
            }

            public int GetSwOut(int port) {
                return extractPortValue(SW_OUT_START_BYTE, port, min_port, max_port);
            }

            //-----------------PUBLIC SETTERS---------------------

            public void SetDefaultPortType() {
                for (int i = 0; i < max_port; i++) {
                    SetPortType(i, prot_type_ARTNET_in_out);
                }
            }
            public void SetPortType(int port, int type) {
                if (port >= 0 && port < 4) {
                    data[PORT_TYPE_START_BYTE + port] = (byte)type;
                }
            }

            public void SetDefaultGoodInput() {
                for (int i = 0; i < max_port; i++) {
                    SetGoodInput(i, good_input_DEFAULT);
                }
            }
            public void SetGoodInput(int port, int value) {
                if (port >= 0 && port < max_port) {
                    data[GOOD_INPUT_START_BYTE + port] = (byte)value;
                }
            }

            public void SetDefaultGoodOutput() {
                for (int i = 0; i < max_port; i++) {
                    SetGoodOutput(i, good_output_DEFAULT);
                }
            }
            public void SetGoodOutput(int port, int value) {
                if (port >= 0 && port < 4) {
                    data[GOOD_OUTPUT_START_BYTE + port] = (byte)value;
                }
            }

            public void SetDefaultSwIn() {
                for (int i = 0; i < max_port; i++) {
                    SetSwIn(i, 0);
                }
            }
            public void SetSwIn(int port, int value) {
                if (port >= 0 && port < 4) {
                    data[SW_IN_START_BYTE + port] = (byte)value;
                }
            }

            public void SetDefaultSwOut() {
                for (int i = 0; i < max_port; i++) {
                    SetSwOut(i, i); // set out ports to 1, 2, 3, 4;
                }
            }
            public void SetSwOut(int port, int value) {
                if (port >= 0 && port < 4) {
                    data[SW_OUT_START_BYTE + port] = (byte)value;
                }
            }

            //ATENTION! this method does not have coresponding getter method, if you provide it - delete this comment.
            public void setSwVideo_SwMacro_SwRemote_to_zero() { // see Art-Net 14 Documentation 
                data[SW_VIDEO_START_BYTE] = (byte)0;
                data[SW_MACRO_START_BYTE] = (byte)0;
                data[SW_REMOTE_START_BYTE] = (byte)0;
            }

            //ATENTION! this method does not have coresponding getter method, if you provide it - delete this comment.
            public void setSpareFieldsToZero() { // see Art-Net 14 Documentation
                data[SPARE1_START_BYTE] = (byte)0;
                data[SPARE2_START_BYTE] = (byte)0;
                data[SPARE3_START_BYTE] = (byte)0;
            }

            //------------------PRIVATE SETTERS------------------

            private void setMacAddress() {
                this.MacAddress = Const.GetMacAddressOfLocalMachine();
            }

            private void setFiller() {
                for (int i = 0; i < data.Length - FILLER_START_BYTE; i++) {
                    data[FILLER_START_BYTE + i] = (byte)0;
                }
            }



            //------------------PRIVATE METHODS------------------
            private string extractString(int startByte, int nameLength) {
                int endIndex = Const.FindIndexOfValueInByteArray(
                        0, data, startByte, nameLength); 
                if (endIndex == -1) { //index not found case
                    Console.WriteLine("value not found at ArtPollReply.ShortName {get}");
                    endIndex = 0;
                }
                return Const.GetASCIIFromByteArray(data, startByte, endIndex);
            }


            private int extractPortValue(int startByte, int port, int rangeMin, int rangeMax) {
                if (port >= rangeMin && port >= rangeMax) {
                    return Const.ToUnsignedInt32Value(data[startByte + (port - 1)]);
                }
                return -1; //port out of range return value
            }

            private byte[] extractAddressByteArray(int startByte, int length) {
                var address = new byte[length]; 
                for (int i = 0; i < address.Length; i++) {
                    address[i] = data[startByte + i];
                }
                return address;
            }

            private string extractAddressString(byte[] byteAddress, string devider) {
                StringBuilder sb = new StringBuilder();
                var address = byteAddress;
                for (int i = 0; i < address.Length; i++) {
                    if (devider == mac_devider)
                        sb.Append(
                            Const.ToUnsignedInt32Value(address[i]).
                            ToString("X") + devider);
                    else
                        sb.Append(
                            Const.ToUnsignedInt32Value(address[i]).
                            ToString() + devider);
                }
                return sb.ToString().Substring(0, sb.Length - 1); //remove last devider and return
            }
        }
    }
}
