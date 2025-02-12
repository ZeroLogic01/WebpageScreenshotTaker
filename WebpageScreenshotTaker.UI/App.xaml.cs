using System.Windows;

namespace WebpageScreenshotTaker.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles any unhandled exception at application level. It will prevent application crash from the exception.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is System.Runtime.InteropServices.COMException comException && comException.ErrorCode == -2147221040)
                e.Handled = true;

            if (e.Exception != null &&
                MessageBox.Show($"An unexpected error occurred: {e.Exception}", "Do you want to close the application?", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            e.Handled = true;
        }
    }
}
