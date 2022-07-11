namespace DhcpStuff
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading.Tasks;

    public class Program
    {
        static async Task Main(string[] args)
        {
            var options = new RelayOptions
            {
                InterfaceNames = new[] { "Ethernet", },
                Servers = new[] { "10.0.1.1", },
            };

            var servers = options.Servers.Select(s => IPAddress.Parse(s)).ToArray();

            var tasks = new List<Task>(options.InterfaceNames.Length);
            foreach (var inf in options.InterfaceNames)
            {
                var netinf = NetworkInterface
                    .GetAllNetworkInterfaces()
                    .FirstOrDefault(nic => nic.Name.Equals(inf, StringComparison.CurrentCultureIgnoreCase));

                var relay = new Relay(netinf, options);
                tasks.Add(relay.Run());
            }

            await Task.WhenAll(tasks);
        }
    }
}
