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
            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting HWID: {ex.Message}");
                return "UNKNOWN_HWID";
            }
        }

        private static string GetCpuId()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        var processorId = obj["ProcessorId"];
                        if (processorId != null)
                            return processorId.ToString();
                    }
                }
                return "CPU_UNKNOWN";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting CPU ID: {ex.Message}");
                return "CPU_ERROR";
            }
        }

        private static string GetVolumeSerial()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_LogicalDisk WHERE DeviceID='C:'"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        var serialNumber = obj["SerialNumber"];
                        if (serialNumber != null)
                            return serialNumber.ToString();
                    }
                }
                return "VOL_UNKNOWN";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting volume serial: {ex.Message}");
                return "VOL_ERROR";
            }
        }

        private static string GetMacAddress()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT MACAddress FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled=True"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        var macAddress = obj["MACAddress"];
                        if (macAddress != null)
                            return macAddress.ToString();
                    }
                }
                return "MAC_UNKNOWN";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting MAC address: {ex.Message}");
                return "MAC_ERROR";
            }
        }
    }
}
