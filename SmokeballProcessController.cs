using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace JC.QuickMove
{
    internal class SmokeballProcessController
    {
        [Flags]
        public enum SmokeballProcess { Main = 1, Service = 2 }

        public void Kill(SmokeballProcess processToKill = SmokeballProcess.Main | SmokeballProcess.Service)
        {
            if (processToKill.HasFlag(SmokeballProcess.Main)) 
            {
                Process process = Process.GetProcessesByName("Smokeball").FirstOrDefault();

                try
                {
                    if (process != null)
                    {
                        Console.WriteLine("Smokeball.exe stopped");
                        process.Kill();
                    }
                    else
                    {
                        Console.WriteLine("Unable to find Smokeball.exe");
                    }
                }
                catch(Exception ex) 
                {
                    Console.WriteLine($"Error stopping Smokeball.exe : {ex.Message}");
                }
            }
            
            if (processToKill.HasFlag(SmokeballProcess.Service)) 
            {
                ServiceController service = new ServiceController("Smokeball-Windows-Service");

                if (service != null)
                {
                    if (service.Status == ServiceControllerStatus.Running || service.Status == ServiceControllerStatus.StartPending)
                    {
                        Console.WriteLine("Stopping service");
                        service.Stop();
                    }
                }
                else
                {
                    Console.WriteLine("Unable to find Smokeball Service");
                }
            }
        }

        public void Start(SmokeballProcess processToStart = SmokeballProcess.Main | SmokeballProcess.Service)
        {
            if (processToStart.HasFlag(SmokeballProcess.Main))
            {
                Process process = Process.GetProcessesByName("Smokeball").FirstOrDefault();

                try
                {
                    if (process != null)
                    {
                        Console.WriteLine("Smokeball.exe already running");
                        process.Kill();
                    }
                    else
                    {
                        Process.Start(new ProcessStartInfo("\"C:\\Program Files\\Smokeball\\binaries\\Smokeball.exe\""));
                        Console.WriteLine("Smokeball.exe restarted");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error starting Smokeball.exe : {ex.Message}");
                }
            }

            if (processToStart.HasFlag(SmokeballProcess.Service))
            {
                ServiceController service = new ServiceController("Smokeball-Windows-Service");

                if (service != null)
                {
                    if (service.Status == ServiceControllerStatus.Running || service.Status == ServiceControllerStatus.StartPending)
                    {
                        service.Start();
                        Console.WriteLine("Service restarted");
                    }
                }
                else
                {
                    Console.WriteLine("Unable to find Smokeball Service");
                }
            }
        }
    }
}
