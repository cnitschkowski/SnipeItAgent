using System;

namespace SnipeItAgent
{
    public abstract class SystemInfoProviderBase : ISystemInfoProvider
    {
        public SystemInfo GetSystemInfo()
        {
            return this.GetSystemInfoInternal();
        }

        protected virtual SystemInfo GetSystemInfoInternal()
        {
            var info = new SystemInfo
            {
                Hostname = Environment.MachineName,
                OperatingSystem = Environment.OSVersion
            };

            return info;
        }
    }
}