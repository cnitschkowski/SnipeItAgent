using System;

namespace SnipeItAgent
{
    public class SystemInfo
    {
        public string Manufacturer { get; set; }
        
        public string Model { get; set; }
        
        public string SerialNumber { get; set; }
        
        public string Hostname { get; set; }
        
        public ulong Memory { get; set; }
        
        public string Platform{ get; set; }
        
        public bool IsNotebook { get; set; }
        
        public bool IsClient { get; set; }
        
        public OperatingSystem OperatingSystem { get; set; }
    }
}