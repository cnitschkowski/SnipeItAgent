using System;
using System.IO;
using System.Linq;

namespace SnipeItAgent
{
    public class LinuxSystemInfoProvider : SystemInfoProviderBase
    {
        protected override SystemInfo GetSystemInfoInternal()
        {
            var info = base.GetSystemInfoInternal();

            var infoBasePath = "/sys/devices/virtual/dmi/id";

            info.Manufacturer = File.ReadAllText(Path.Combine(infoBasePath, "sys_vendor")).Trim();
            info.Model = File.ReadAllText(Path.Combine(infoBasePath, "product_name")).Trim();

            try
            {
                info.SerialNumber = File.ReadAllText(Path.Combine(infoBasePath, "product_serial")).Trim();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }

            var memInfo = File.ReadAllLines("/proc/meminfo").First();
            var segments = memInfo.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var memory = segments[1];

            info.Memory = Convert.ToUInt64(memory) * 1024;
            
            return info;
        }
    }
}