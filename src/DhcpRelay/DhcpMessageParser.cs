namespace DhcpStuff
{
    using System;
    using System.Text;

    public static class DhcpMessageParser
    {
        public static DhcpMessage2 FromBytes(byte[] bytes)
        {
            return new DhcpMessage2
            {
                Operation = (Operation)bytes[0],
                HardwareAddressType = bytes[1],
                HardwareAddressLength = bytes[2],
                Hops = bytes[3],
                TransactionId = (uint)BitConverter.ToInt32(bytes, 4),
                Seconds = BitConverter.ToInt16(bytes, 8),
                Flags = BitConverter.ToInt16(bytes, 10),
                ClientIpAddress = (uint)BitConverter.ToInt32(bytes, 12),
                YourIpAddress = (uint)BitConverter.ToInt32(bytes, 16),
                ServerIpAddress = (uint)BitConverter.ToInt32(bytes, 20),
                GatewayIpAddress = (uint)BitConverter.ToInt32(bytes, 24),
                ClientHardwareAddress = (uint)BitConverter.ToInt32(bytes, 28),
                ServerHostName = GetString(bytes, 44, 64),
                File = GetString(bytes, 108, 128),
                Options = bytes[236..^0],
            };
        }

        public static byte[] ToBtyes(DhcpMessage2 message)
        {
            var bytes = new byte[236 + message.Options.Length];
            bytes[0] = (byte)message.Operation;
            bytes[1] = message.HardwareAddressType;
            bytes[2] = message.HardwareAddressLength;
            bytes[3] = message.Hops;
            Array.Copy(BitConverter.GetBytes(message.TransactionId), 0, bytes, 4, 4);
            Array.Copy(BitConverter.GetBytes(message.Seconds), 0, bytes, 8, 2);
            Array.Copy(BitConverter.GetBytes(message.Flags), 0, bytes, 10, 2);
            Array.Copy(BitConverter.GetBytes(message.ClientIpAddress), 0, bytes, 12, 4);
            Array.Copy(BitConverter.GetBytes(message.YourIpAddress), 0, bytes, 16, 4);
            Array.Copy(BitConverter.GetBytes(message.ServerIpAddress), 0, bytes, 20, 4);
            Array.Copy(BitConverter.GetBytes(message.GatewayIpAddress), 0, bytes, 24, 4);
            Array.Copy(BitConverter.GetBytes(message.ClientHardwareAddress), 0, bytes, 28, 4);
            Array.Copy(ToBytes(message.ServerHostName, 64), 0, bytes, 44, 64);
            Array.Copy(ToBytes(message.File, 64), 0, bytes, 108, 64);
            Array.Copy(message.Options, 0, bytes, 236, message.Options.Length);

            return bytes;
        }

        private static string GetString(byte[] bytes, int startIndex, int maxLength)
        {
            var x = 0;
            while (x <= maxLength && bytes[startIndex + x] != 0)
            {
                x++;
            }

            return Encoding.Default.GetString(bytes, startIndex, x);
        }

        private static byte[] ToBytes(string source, int totalLength)
        {
            if (source.Length >= totalLength)
            {
                throw new ArgumentOutOfRangeException(nameof(source), source.Length, $"The length of the string must be less than {totalLength}.");
            }

            var bytes = new byte[totalLength];

            Array.Copy(Encoding.Default.GetBytes(source), bytes, source.Length);

            for (var x = source.Length; x <= totalLength; x++)
            {
                bytes[x] = 0;
            }

            return bytes;
        }

        // https://www.ietf.org/rfc/rfc2131.txt
        /*
           0                   1                   2                   3
           0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
           +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
           |     op (1)    |   htype (1)   |   hlen (1)    |   hops (1)    |
           +---------------+---------------+---------------+---------------+
           |                            xid (4)                            |
           +-------------------------------+-------------------------------+
           |           secs (2)            |           flags (2)           |
           +-------------------------------+-------------------------------+
           |                          ciaddr  (4)                          |
           +---------------------------------------------------------------+
           |                          yiaddr  (4)                          |
           +---------------------------------------------------------------+
           |                          siaddr  (4)                          |
           +---------------------------------------------------------------+
           |                          giaddr  (4)                          |
           +---------------------------------------------------------------+
           |                                                               |
           |                          chaddr  (16)                         |
           |                                                               |
           |                                                               |
           +---------------------------------------------------------------+
           |                                                               |
           |                          sname   (64)                         |
           +---------------------------------------------------------------+
           |                                                               |
           |                          file    (128)                        |
           +---------------------------------------------------------------+
           |                                                               |
           |                          options (variable)                   |
           +---------------------------------------------------------------+

                  Figure 1:  Format of a DHCP message



            FIELD      OCTETS       DESCRIPTION
            -----      ------       -----------

            op            1  Message op code / message type.
                            1 = BOOTREQUEST, 2 = BOOTREPLY
            htype         1  Hardware address type, see ARP section in "Assigned
                            Numbers" RFC; e.g., '1' = 10mb ethernet.
            hlen          1  Hardware address length (e.g.  '6' for 10mb
                            ethernet).
            hops          1  Client sets to zero, optionally used by relay agents
                            when booting via a relay agent.
            xid           4  Transaction ID, a random number chosen by the
                            client, used by the client and server to associate
                            messages and responses between a client and a
                            server.
            secs          2  Filled in by client, seconds elapsed since client
                            began address acquisition or renewal process.
            flags         2  Flags (see figure 2).
            ciaddr        4  Client IP address; only filled in if client is in
                            BOUND, RENEW or REBINDING state and can respond
                            to ARP requests.
            yiaddr        4  'your' (client) IP address.
            siaddr        4  IP address of next server to use in bootstrap;
                            returned in DHCPOFFER, DHCPACK by server.
            giaddr        4  Relay agent IP address, used in booting via a
                            relay agent.
            chaddr       16  Client hardware address.
            sname        64  Optional server host name, null terminated string.
            file        128  Boot file name, null terminated string; "generic"
                            name or null in DHCPDISCOVER, fully qualified
                            directory-path name in DHCPOFFER.
            options     var  Optional parameters field.  See the options
                            documents for a list of defined options.

                Table 1:  Description of fields in a DHCP message




                                    1 1 1 1 1 1
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |B|             MBZ             |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                B:  BROADCAST flag

                MBZ:  MUST BE ZERO (reserved for future use)

                Figure 2:  Format of the 'flags' field

        */
    }
}

