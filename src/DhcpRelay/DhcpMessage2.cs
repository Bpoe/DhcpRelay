namespace DhcpStuff
{
    public class DhcpMessage2
    {
        /// <summary>
        /// Gets or sets the op field.
        /// </summary>
        public Operation Operation { get; set; }

        /// <summary>
        /// Gets or sets the htype field.
        /// </summary>
        public byte HardwareAddressType { get; set; }

        /// <summary>
        /// Gets or sets the hlen field.
        /// </summary>
        public byte HardwareAddressLength { get; set; }

        /// <summary>
        /// Gets or sets the hops field.
        /// </summary>
        public byte Hops { get; set; }

        /// <summary>
        /// Gets or sets the xid field.
        /// </summary>
        public uint TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the secs field.
        /// </summary>
        public short Seconds { get; set; }

        /// <summary>
        /// Gets or sets the flags field.
        /// </summary>
        public short Flags { get; set; }

        /// <summary>
        /// Gets or sets the ciaddr field.
        /// </summary>
        public uint ClientIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the yiaddr field.
        /// </summary>
        public uint YourIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the siaddr field.
        /// </summary>
        public uint ServerIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the giaddr field.
        /// </summary>
        public uint GatewayIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the chaddr field.
        /// </summary>
        public uint ClientHardwareAddress { get; set; }

        /// <summary>
        /// Gets or sets the sname field.
        /// </summary>
        public string ServerHostName { get; set; }

        /// <summary>
        /// Gets or sets the file field.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Gets the options field.
        /// </summary>
        public byte[] Options { get; set; }

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
