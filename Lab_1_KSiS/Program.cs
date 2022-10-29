using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Lab_1_KSiS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            Console.WriteLine("Information about system for {0}.{1}     \n", computerProperties.HostName, computerProperties.DomainName);
            ShowMacAddress();
            ShowShareNames();
        }

        private static void ShowMacAddress()
        {
            NetworkInterface[] names = NetworkInterface.GetAllNetworkInterfaces();
            if (names == null || names.Length < 1)
            {
                Console.WriteLine("No network interfaces found.");
                return;
            }

            foreach (NetworkInterface adapter in names)
            {
                if (!string.IsNullOrEmpty(adapter.GetPhysicalAddress().ToString()))
                {
                    Console.WriteLine($"{adapter.Description}\n{String.Join("-", adapter.GetPhysicalAddress().GetAddressBytes())}");
                    Console.WriteLine();
                }
            }
        }

        private static void ShowShareNames()
        {
            Console.WriteLine("Working groups and network computers:");
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    Arguments = "/c net view",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            proc.Start();
            PrintShareNames(proc);
        }

        private static void PrintShareNames(Process proc)
        {
            string data = proc.StandardOutput.ReadToEnd();

            if (data == null || data == "")
            {
                Console.WriteLine("No working groups and network computers found.");
                return;
            }

            int start = 0;
            while (true)
            {
                start = data.IndexOf('\\', start);
                if (start == -1)
                    break;
                var stop = data.IndexOf('\n', start);
                Console.WriteLine("   {0}", data.Substring(start, stop - start));
                start = stop;
            }
        }
    }
}
