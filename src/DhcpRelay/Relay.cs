namespace DhcpStuff
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class Relay
    {
        private const int DhcpServerPort = 67;
        private const int DhcpClientPort = 68;

        private static readonly byte[] NullIpAddress = new byte[] { 0, 0, 0, 0 };

        private readonly NetworkInterface nic;
        private readonly RelayOptions options;
        private readonly IPAddress ipAddress;
        private readonly IPEndPoint[] serverEndpoints;
        private readonly IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Broadcast, DhcpClientPort);

        private UdpClient sender;

        public Relay(NetworkInterface nic, RelayOptions options)
        {
            this.nic = nic ?? throw new ArgumentNullException(nameof(nic));
            this.options = options ?? throw new ArgumentNullException(nameof(options));

            var serverIpAddresses = options.Servers.Select(s => IPAddress.Parse(s));
            this.serverEndpoints = serverIpAddresses.Select(s => new IPEndPoint(s, DhcpServerPort)).ToArray();

            this.ipAddress = nic
                .GetIPProperties()
                .UnicastAddresses
                .FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Address;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            var listener = new UdpClient(new IPEndPoint(this.ipAddress, DhcpServerPort));
            this.sender = new UdpClient(new IPEndPoint(this.ipAddress, DhcpClientPort));

            var infMac = BitConverter
                .ToString(this.nic.GetPhysicalAddress().GetAddressBytes())
                .Replace('-', ':')
                .ToLowerInvariant();
            Console.WriteLine("Listening on {0}/{1}", this.nic.Name, infMac);

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await listener.ReceiveAsync();
                await this.HandleMessage(result);
            }
        }

        public async Task HandleMessage(UdpReceiveResult result)
        {
            var message = new DhcpMessage(result.Buffer);

            switch (message.Operation)
            {
                case Operation.BootRequest:
                    await this.HandleBootRequest(message);
                    break;
                case Operation.BootReply:
                    await this.HandleBootReply(message);
                    break;
            }
        }

        public async Task HandleBootRequest(DhcpMessage message)
        {
            if (message.HardwareAddressLength > 16)
            {
                Console.WriteLine(
                    "Discarding packet with invalid hlen, received on {0} interface.",
                    this.nic.Name);

                return;
            }

            if (message.Hops > this.options.MaxHopCount)
            {
                Console.WriteLine("Discarding packet that has exceeded the max hop count.");
                return;
            }

            message.Hops++;

            if (message.GatewayIpAddress.All(b => b == 0))
            {
                message.GatewayIpAddress = this.ipAddress.GetAddressBytes();
            }

            // Send to DHCP servers
            var payload = message.GetBytes();
            foreach (var server in this.serverEndpoints)
            {
                await this.sender.SendAsync(payload, payload.Length, server);

                Console.WriteLine(
                    "Forwarded {0} for {1} to {2}",
                    message.Operation.ToString().ToUpperInvariant(), 
                    GetHardwareAddressString(message), 
                    server.Address.ToString());
            }
        }

        public async Task HandleBootReply(DhcpMessage message)
        {
            // Clear the giaddr
            message.GatewayIpAddress = NullIpAddress;

            // Send to DHCP client via broadcast

            // .Net doesn't seem to be able to set the L2 address. This
            // prevents us from using a unicast reply to the new IP using
            // the MAC address. We MUST use the broadcast address.
            var payload = message.GetBytes();
            await this.sender.SendAsync(payload, payload.Length, this.clientEndpoint);

            Console.WriteLine(
                "Forwarded {0} for {1} to {2}",
                message.Operation.ToString().ToUpperInvariant(),
                GetHardwareAddressString(message),
                this.clientEndpoint.Address.ToString());
        }

        private static string GetHardwareAddressString(DhcpMessage message)
        {
            return BitConverter
                .ToString(message.ClientHardwareAddress, 0, message.HardwareAddressLength)
                .Replace('-', ':')
                .ToLowerInvariant();
        }
    }
}
