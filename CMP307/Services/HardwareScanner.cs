using System;
using System.Management;
using CMP307.Models;

namespace CMP307.Services
{
    public static class HardwareScanner
    {
        public static Hardware ScanHardware()
        {
            var hardware = new Hardware();

            // Get system name
            hardware.SystemName = Environment.MachineName;

            // Get model and manufacturer
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (var obj in searcher.Get())
                {
                    hardware.Model = obj["Model"]?.ToString();
                    hardware.Manufacturer = obj["Manufacturer"]?.ToString();
                }
            }

            // Get type (e.g., Desktop, Laptop)
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SystemEnclosure"))
            {
                foreach (var obj in searcher.Get())
                {
                    var chassisTypes = (ushort[])obj["ChassisTypes"];
                    if (chassisTypes != null && chassisTypes.Length > 0)
                    {
                        hardware.Type = GetChassisType(chassisTypes[0]);
                    }
                }
            }

            // Get IP address
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE"))
            {
                foreach (var obj in searcher.Get())
                {
                    var addresses = (string[])obj["IPAddress"];
                    if (addresses != null && addresses.Length > 0)
                    {
                        hardware.IpAddress = addresses[0];
                        break;
                    }
                }
            }

            return hardware;
        }

        private static string GetChassisType(ushort type)
        {
            return type switch
            {
                3 => "Desktop",
                4 => "Low Profile Desktop",
                5 => "Pizza Box",
                6 => "Mini Tower",
                7 => "Tower",
                8 => "Portable",
                9 => "Laptop",
                10 => "Notebook",
                11 => "Hand Held",
                12 => "Docking Station",
                13 => "All in One",
                14 => "Sub Notebook",
                15 => "Space-Saving",
                16 => "Lunch Box",
                17 => "Main System Chassis",
                18 => "Expansion Chassis",
                19 => "SubChassis",
                20 => "Bus Expansion Chassis",
                21 => "Peripheral Chassis",
                22 => "Storage Chassis",
                23 => "Rack Mount Chassis",
                24 => "Sealed-Case PC",
                _ => "Unknown",
            };
        }
    }
}
