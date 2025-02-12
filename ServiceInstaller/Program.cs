using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace ServiceInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                string appRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

                using (Process cmd = new Process())
                {
                    cmd.StartInfo.FileName = "cmd.exe";

                    cmd.StartInfo.RedirectStandardInput = true;
                    cmd.StartInfo.RedirectStandardOutput = true;
                    cmd.StartInfo.RedirectStandardError = true;

                    cmd.StartInfo.Verb = "runas";
                    cmd.StartInfo.CreateNoWindow = false;
                    cmd.StartInfo.UseShellExecute = false;
                    cmd.Start();

                    if (StopService("WebpageScreenshotTakerService$WebpageScreenshotTaker", 20000))
                    {
                        Console.WriteLine("Deleting the old windows service...");
                        cmd.StandardInput.WriteLine($"sc delete WebpageScreenshotTakerService$WebpageScreenshotTaker");
                        cmd.StandardInput.Flush();
                    }
                    Console.WriteLine("Installing the the latest version...");
                    cmd.StandardInput.WriteLine($"cd {appRoot}");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.WriteLine($"WebpageScreenshotTaker.Service.exe install start");
                    cmd.StandardInput.Flush();
                    cmd.StandardInput.Close();
                    cmd.WaitForExit();
                    Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                    Console.WriteLine(cmd.StandardError.ReadToEnd());
                }
            }
            catch { }
        }

        public static bool StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == serviceName);
            try
            {
                if (service == null)
                {
                    return false;
                }
                else if (service.Status == ServiceControllerStatus.Running)
                {

                    Console.WriteLine(service.Status);
                    Console.WriteLine("Stopping the old windows service...");

                    TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return false;
        }
    }
}
