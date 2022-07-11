namespace DhcpStuff
{
    public class RelayOptions
    {
        public string[] InterfaceNames { get; set; }

        public string[] Servers { get; set; }

        public int Port { get; set; } = 67;

        public int MaxHopCount { get; set; } = 4;
    }
}
