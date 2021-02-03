using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mutey.Output;

namespace Discord.Trial
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Process> discordProcesses = Process.GetProcesses()
                .Where(p=>p.MainWindowHandle!=IntPtr.Zero)
                .Where(p=>p.ProcessName.IndexOf("Discord", StringComparison.OrdinalIgnoreCase) >=0)
                .ToList();

            if (discordProcesses.Count == 0)
            {
                Console.WriteLine("Discord is not running...");
                Console.WriteLine("Exitting trial...");
                
                return;
            }

            if (discordProcesses.Count > 1)
            {
                Console.WriteLine($"Multiple instances of Discord detected, testing first instance found ({discordProcesses[0].Id})");
            }

            Process process = discordProcesses[0];

            SendKeysCall call = new SendKeysCall("Discord", process, 
                SendKeysCall.LEFT_ALT,
                SendKeysCall.LEFT_CTRL,
                SendKeysCall.M);

            Console.WriteLine("Call created, press enter to send toggle command...");

            while (true)
            {
                string line = Console.ReadLine();
                if (line == null || line.Length == 0)
                {
                    Console.WriteLine("Sending toggle...");
                    call.Toggle();
                    Console.WriteLine("Toggle sent");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Exitting trial...");
                    return;
                }
            }
        }
    }
}