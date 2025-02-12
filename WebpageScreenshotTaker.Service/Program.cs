using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Reflection;
using Topshelf;

namespace ScreenshotTaker.Service
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            FileInfo configFileInfo = new FileInfo("log4net.config");


            GlobalContext.Properties["LogFileRootPath"] = Path.GetPathRoot(Environment.SystemDirectory);
            XmlConfigurator.Configure(configFileInfo);

            var rc = HostFactory.Run(x =>
            {
                x.SetServiceName("WebpageScreenshotTakerService");
                x.SetDisplayName("Webpage Screenshot Taker Service");
                x.SetDescription("This is a screenshot taker service to capture " +
                    "screenshot of the webpages at scheduled time each day.");
                x.SetInstanceName("WebpageScreenshotTaker");

                x.Service<WebpageScreenshotService>();

                x.OnException(ex =>
                {
                    _log.Error(ex.Message, ex);
                });

                // Enable service recovery
                x.EnableServiceRecovery(r =>
                {
                    r.OnCrashOnly();
                    r.RestartService(1); //first
                    r.RestartService(1); //second
                    r.RestartService(1); //subsequents
                    r.SetResetPeriod(0);
                });

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.UseLog4Net();
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}
