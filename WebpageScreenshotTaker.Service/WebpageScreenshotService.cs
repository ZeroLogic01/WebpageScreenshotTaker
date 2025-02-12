using log4net;
using Quartz;
using Quartz.Impl;
using ScreenshotTaker.Service.Jobs;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Topshelf;

namespace ScreenshotTaker.Service
{
    public class WebpageScreenshotService : ServiceControl
    {
        #region Fields
        private readonly IScheduler _scheduler;
        private static readonly ILog _log = LogManager.GetLogger(typeof(WebpageScreenshotService));

        #endregion

        public WebpageScreenshotService()
        {
            //Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
            {
                ["quartz.serializer.type"] = "binary",
                ["quartz.scheduler.instanceName"] = "WebpageScreenshotScheduler",
                // Set thread count to 1 to force Triggers scheduled for the same time to
                // to be ordered by priority.
                ["quartz.threadPool.threadCount"] = "10",
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz"
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            _scheduler = Task.Run(() => factory.GetScheduler()).Result;

        }

        public bool Start(HostControl hostControl)
        {
            //string[] lines = new string[] { ConnectionString.Get() };
            //File.AppendAllLines(@"C:\Logs\Logs.txt", lines);
            // start the scheduler
            Task.Run(() => { _scheduler.Start(); }).Wait();

            IJobDetail job = JobBuilder
                    .Create<WebpageScreenshotJob>()
                    .WithIdentity(typeof(WebpageScreenshotJob).Name, SchedulerConstants.DefaultGroup)
                    .Build();

            ITrigger trigger = TriggerBuilder
                                .Create()
                                .WithIdentity("simpletrigger", SchedulerConstants.DefaultGroup)
                                .ForJob(job)
                                .StartNow()
                                .WithSimpleSchedule(s => s
                                    .WithIntervalInMinutes(1)
                                    .RepeatForever()
                                    .WithMisfireHandlingInstructionFireNow())
                                .Build();
            Task.Run(() => { _scheduler.ScheduleJob(job, trigger); }).Wait();

            return true;
        }


        public bool Stop(HostControl hostControl)
        {
            _log.Info("Stopping the service...");
            Task.Run(() => _scheduler.Shutdown()).Wait();
            return true;
        }

    }
}
