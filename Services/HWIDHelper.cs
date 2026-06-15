using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace ScholasticaReader.Services
{
    public static class HWIDHelper
    {
        public static string GetHWID()
        {
            string cpuId = GetCpuId();
            string volumeSerial = GetVolumeSerial();
            string mac = GetMacAddress();

            string combined = $"{cpuId}-{volumeSerial}-{mac}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToHexString(hash).Substring(0, 16);
            }
        }

        private static string GetCpuId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (var obj in searcher.Get())
                        return obj["ProcessorId"]?.ToString() ?? "CPU_UNKNOWN";
                }
            }
            catch { return "CPU_ERROR"; }
            return "CPU_ERROR";
        }

        private static string GetVolumeSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_LogicalDisk WHERE DeviceID='C:'"))
                {
                    foreach (var obj in searcher.Get())
                        return obj["SerialNumber"]?.ToString() ?? "VOL_UNKNOWN";
                }
            }
            catch { return "VOL_ERROR"; }
            return "VOL_ERROR";
        }

        private static string GetMacAddress()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=True"))
                {
                    foreach (var obj in searcher.Get())
                        return obj["MACAddress"]?.ToString() ?? "MAC_UNKNOWN";
                }
            }
            catch { return "MAC_ERROR"; }
            return "MAC_ERROR";
        }
    }
}
