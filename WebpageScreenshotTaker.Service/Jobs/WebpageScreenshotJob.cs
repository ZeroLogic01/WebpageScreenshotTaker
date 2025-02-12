using log4net;
using Quartz;
using ScreenshotTaker.Service.Jobs.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebpageScreenshotTaker.Data;

namespace ScreenshotTaker.Service.Jobs
{
    public class WebpageScreenshotJob : IJob
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(WebpageScreenshotJob));

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string timeNow = DateTime.Now.ToLocalTime().ToShortTimeString();
                //_log.Error("An unhandled exception occured inside sceduler"+ timeNow);


                _log.Info($"Start Time: {timeNow}");

                var timeScheduled = new { TimeScheduled = timeNow };

                var screenshotDetailsList = await DAL.GetScheduledScreenshotDetails(timeScheduled);

                if (screenshotDetailsList?.Count <= 0)
                {
                    return;
                }

                // divide list in chunks (10 is default chunk size)
                var sublists = ListSplitter.SplitList(screenshotDetailsList);
                foreach (var list in sublists)
                {
                    var tasks = from screenshot in list
                                select ScreenshotCapturer
                                .DownloadWebpageScreenshot(screenshot.URL, screenshot.Directory, screenshot.ImageName);

                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}
