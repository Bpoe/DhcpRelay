namespace DhcpStuff
{
    using System;
    using System.Text;

    public class DhcpMessage
    {
        private readonly byte[] bytes;

        public DhcpMessage(byte[] bytes)
        {
            this.bytes = bytes;
        }

        public byte[] GetBytes()
        {
            return this.bytes;
        }

        /// <summary>
        /// Gets or sets the op field.
        /// </summary>
        public Operation Operation
        {
            get
            {
                return (Operation)this.bytes[0];
            }

            set
            {
                this.bytes[0] = (byte)value;
            }
        }

        /// <summary>
        /// Gets or sets the htype field.
        /// </summary>
        public byte HardwareAddressType
        {
            get
            {
                return this.bytes[1];
            }

            set
            {
                this.bytes[1] = value;
            }
        }

        /// <summary>
        /// Gets or sets the hlen field.
        /// </summary>
        public byte HardwareAddressLength
        {
            get
            {
                return this.bytes[2];
            }

            set
            {
                this.bytes[2] = value;
            }
        }

        /// <summary>
        /// Gets or sets the hops field.
        /// </summary>
        public byte Hops
        {
            get
            {
                return this.bytes[3];
            }

            set
            {
                this.bytes[3] = value;
            }
        }

        /// <summary>
        /// Gets or sets the xid field.
        /// </summary>
        public uint TransactionId
        {
            get
            {
                return (uint)BitConverter.ToInt32(this.bytes, 4);
            }

            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.bytes, 4, 4);
            }
        }

        /// <summary>
        /// Gets or sets the secs field.
        /// </summary>
        public short Seconds
        {
            get
            {
                return BitConverter.ToInt16(this.bytes, 8);
            }

            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.bytes, 8, 2);
            }
        }

        /// <summary>
        /// Gets or sets the flags field.
        /// </summary>
        public short Flags
        {
            get
            {
                return BitConverter.ToInt16(this.bytes, 10);
            }

            set
            {
                Array.Copy(BitConverter.GetBytes(value), 0, this.bytes, 10, 2);
            }
        }

        /// <summary>
        /// Gets or sets the ciaddr field.
        /// </summary>
        public byte[] ClientIpAddress

        {
            get
            {
                return this.bytes[12..16];
            }

            set
            {
                if (value.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the byte array must be 4.");
                }

                Array.Copy(value, 0, this.bytes, 12, 4);
            }
        }

        /// <summary>
        /// Gets or sets the yiaddr field.
        /// </summary>
        public byte[] YourIpAddress
        {
            get
            {
                return this.bytes[16..20];
            }

            set
            {
                if (value.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the byte array must be 4.");
                }

                Array.Copy(value, 0, this.bytes, 16, 4);
            }
        }

        /// <summary>
        /// Gets or sets the siaddr field.
        /// </summary>
        public byte[] ServerIpAddress
        {
            get
            {
                return this.bytes[20..24];
            }

            set
            {
                if (value.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the byte array must be 4.");
                }

                Array.Copy(value, 0, this.bytes, 20, 4);
            }
        }

        /// <summary>
        /// Gets or sets the giaddr field.
        /// </summary>
        public byte[] GatewayIpAddress
        {
            get
            {
                return this.bytes[24..28];
            }

            set
            {
                if (value.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the byte array must be 4.");
                }

                Array.Copy(value, 0, this.bytes, 24, 4);
            }
        }

        /// <summary>
        /// Gets or sets the chaddr field.
        /// </summary>
        public byte[] ClientHardwareAddress
        {
            get
            {
                return this.bytes[28..44];
            }
            
            set
            {
                if (value.Length != 16)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the byte array must be 16.");
                }

                Array.Copy(value, 0, this.bytes, 28, 16);
            }
        }

        /// <summary>
        /// Gets or sets the sname field.
        /// </summary>
        public string ServerHostName
        {
            get
            {
                var x = 0;
                while (x <= 64 && this.bytes[44 + x] != 0)
                {
                    x++;
                }

                return Encoding.Default.GetString(this.bytes, 44, x);
            }

            set
            {
                if (value.Length >= 64)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the string must be less than 64.");
                }

                Encoding.Default.GetBytes(value, 0, value.Length, this.bytes, 44);

                for (var x = 44 + value.Length; x <= 64; x++)
                {
                    this.bytes[x] = 0;
                }
            }
        }

        /// <summary>
        /// Gets or sets the file field.
        /// </summary>
        public string File
        {
            get
            {
                var x = 0;
                while (x <= 128 && this.bytes[108 + x] != 0)
                {
                    x++;
                }

                return Encoding.Default.GetString(this.bytes, 108, x);
            }

            set
            {
                if (value.Length >= 128)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value.Length, "The length of the string must be less than 128.");
                }

                Encoding.Default.GetBytes(value, 0, value.Length, this.bytes, 128);

                for (var x = 108 + value.Length; x <= 128; x++)
                {
                    this.bytes[x] = 0;
                }
            }
        }

        /// <summary>
        /// Gets the options field.
        /// </summary>
        public byte[] Options
        {
            get
            {
                return this.bytes[236..^0];
            }
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
