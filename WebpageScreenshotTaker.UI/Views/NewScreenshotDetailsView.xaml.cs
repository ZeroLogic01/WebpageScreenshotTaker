using MahApps.Metro;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using WebpageScreenshotTaker.Data.Models;

namespace WebpageScreenshotTaker.UI.Views
{
    /// <summary>
    /// Interaction logic for NewScreenshotDetailsView.xaml
    /// </summary>
    public partial class NewScreenshotDetailsView : MetroWindow
    {
        public NewScreenshotDetailsView()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tbTime.Text = DateTime.Now.AddMinutes(2).ToShortTimeString();
            BtnChangeDirectory.Focus();
        }

        private void BtnChangeDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Choose a directory to save the image into"
            };

            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                tbDirectory.Text = dialog.FileName;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbDirectory.Text))
            {
                BtnChangeDirectory.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbTime.Text))
            {
                tbTime.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbImageName.Text))
            {
                tbImageName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(tbUrl.Text))
            {
                tbUrl.Focus();
                return;
            }


            DialogResult = true;

        }


        public ScreenshotDetails NewScreenshot => new ScreenshotDetails()
        {
            Directory = tbDirectory.Text,
            TimeScheduled = Convert.ToDateTime(tbTime.Text),
            ImageName = tbImageName.Text,
            URL = tbUrl.Text
        };

        
    }
}
