

namespace Mf.Core.OperationSystem
{
    public static class OperationSystemService
    {
        public static OperationSystemType OS
        {
            get
            {
                PlatformID platform = Environment.OSVersion.Platform;

                // Check if the operating system is Windows
                if (platform == PlatformID.Win32NT || platform == PlatformID.Win32Windows)
                {
                    Version version = Environment.OSVersion.Version;
                    return OperationSystemType.Windows;
                    // Retrieve and display the Windows version
                }
                else if (platform == PlatformID.Unix)
                {
                    // Retrieve and display the Linux/Unix version
                    //string versionInfo = ExecuteCommand("uname -a");
                    //Console.WriteLine($"Version: {versionInfo}");
                    return OperationSystemType.Linux;
                }
                else
                {
                    return OperationSystemType.Unknown;

                }
            }
        }

        public static string OperationSystemVersion
        {
            get
            {
                return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            }
        }
    }

    public enum OperationSystemType
    {
        Windows,
        Linux,
        Unknown
    }
}
