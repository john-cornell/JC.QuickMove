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
        public enum SmokeballProcess { Main = 1, Service = 2, TrayIcon  = 4 }

        public void Kill(SmokeballProcess processToKill = SmokeballProcess.Main | SmokeballProcess.TrayIcon | SmokeballProcess.Service)
        {
            if (processToKill.HasFlag(SmokeballProcess.Main))
            {
                KillProcess("Smokeball");
            }

            if (processToKill.HasFlag(SmokeballProcess.TrayIcon))
            {
                KillProcess("Smokeball TrayIcon");
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
                        service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0,0,5));

                        if (service.Status != ServiceControllerStatus.Stopped)
                        {
                            Console.WriteLine("Unable to stop service");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Service already stopped");
                    }
                }
                else
                {
                    Console.WriteLine("Unable to find Smokeball Service");
                }
            }
        }

        private static void KillProcess(string processName)
        {
            Process process = Process.GetProcessesByName(processName).FirstOrDefault();

            try
            {
                if (process != null)
                {
                    Console.WriteLine($"{processName}.exe stopped");
                    process.Kill();
                }
                else
                {
                    Console.WriteLine($"Unable to find {processName}.exe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping {processName}.exe : {ex.Message}");
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
                    if (service.Status == ServiceControllerStatus.Stopped || service.Status == ServiceControllerStatus.StopPending)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));

                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            Console.WriteLine("Unable to stop service");
                        }

                        Console.WriteLine("Service restarted");
                    }
                    else
                    {
                        Console.WriteLine("Service already started");
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
