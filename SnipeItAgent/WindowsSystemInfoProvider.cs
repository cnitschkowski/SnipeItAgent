using System;
using System.Linq;
using System.Management;

namespace SnipeItAgent
{
    public class WindowsSystemInfoProvider : SystemInfoProviderBase
    {
        protected override SystemInfo GetSystemInfoInternal()
        {
            var info = base.GetSystemInfoInternal();

            var systemClass = new ManagementClass("Win32_ComputerSystem");
            var systemObject = systemClass.GetInstances().OfType<ManagementObject>().FirstOrDefault();

            if (systemObject != null)
            {
                info.Manufacturer = systemObject["Manufacturer"] as string;
                info.Model = systemObject["Model"] as string;
                info.Memory = (ulong) systemObject["TotalPhysicalMemory"];
            }
            
            info.SerialNumber = GetSerialNumber();

            info.IsNotebook = GetIsNotebook();
            
            info.IsClient = GetIsClient();
 
            info.Platform = GetPlatform();
            
            return info;
        }

        private static bool GetIsNotebook()
        {
            var enclosureClass = new ManagementClass("Win32_SystemEnclosure");
            var enclosureObject = enclosureClass.GetInstances().OfType<ManagementObject>().FirstOrDefault();

            if (enclosureObject == null)
            {
                return false;
            }
            
            var chassisType = (enclosureObject["ChassisTypes"] as ushort[])?.FirstOrDefault();
                
            return chassisType == 9 || chassisType == 10;
        }

        private static bool GetIsClient()
        {
            // FIXME - properly implement this one
            return true;
        }

        private static string GetPlatform()
        {
            var cpuClass = new ManagementClass("Win32_Processor");
            var cpuObject = cpuClass.GetInstances().OfType<ManagementObject>().FirstOrDefault();

            if (cpuObject == null)
            {
                return null;
            }
            
            var cpuType = Convert.ToInt32(cpuObject["Architecture"]);

            if (0.Equals(cpuType))
            {
                return "x86";
            }

            if (5.Equals(cpuType))
            {
                return "arm";
            }

            if (6.Equals(cpuType))
            {
                return "ia64";
            }

            if (9.Equals(cpuType))
            {
                return "x86_64";
            }

            return null;
        }

        private static string GetSerialNumber()
        {
            var biosClass = new ManagementClass("Win32_BIOS");
            var biosObject = biosClass.GetInstances().OfType<ManagementObject>().FirstOrDefault();
            var serialNumber = biosObject?["SerialNumber"] as string;
            return serialNumber;
        }
    }
}