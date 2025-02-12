using System.Windows;

namespace WebpageScreenshotTaker.UI.Helper
{
    public static class MessageBox
    {
        public static void ShowMessageBox(string message, string caption)
        {
            System.Windows.MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
