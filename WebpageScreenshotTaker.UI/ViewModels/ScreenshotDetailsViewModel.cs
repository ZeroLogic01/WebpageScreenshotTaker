using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using WebpageScreenshotTaker.UI.Views;
using WebpageScreenshotTaker.Data;
using WebpageScreenshotTaker.Data.Models;

namespace WebpageScreenshotTaker.UI.ViewModels
{
    public class ScreenshotDetailsViewModel : BindableBase
    {
        #region Public Properties

        public BindingList<ScreenshotDetails> List { get; private set; }

        #region Labels

        public string BtnSelectDirectoryLbl { get; set; } = "---";
        public string BtnAddNewLbl { get; set; } = "ADD";

        #endregion

        #endregion

        #region Contructor

        public ScreenshotDetailsViewModel()
        {
            List = new BindingList<ScreenshotDetails>();
            List.ListChanged += List_ListChanged;

            foreach (var item in DAL.LoadAllData())
            {
                List.Add(item);
            }

            ChangeDirectoryCommand = new DelegateCommand<object>(ChangeScreenshotDirectory, CanChangeScreenshotDirectory);
            // Delete command
            DeleteScreenshotDetailsCommand = new DelegateCommand<object>(DeleteScreenshotDetailsAsync, CanDeleteScreenshotDetails);
            //New Screenshot Command
            NewScreenshotCommand = new DelegateCommand(AddNewScreenshot, () => { return true; });
            //DeleteCommand = new RelayCommand(Delete);
        }

        #endregion

        #region Commands


        #region Change Directory Command

        public DelegateCommand<object> ChangeDirectoryCommand { get; private set; }


        /// <summary>
        /// Changes the directory to save the screenshot of the webpage.
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeScreenshotDirectory(object obj)
        {
            if (obj is ScreenshotDetails screenshot)
            {
                var dialog = new CommonOpenFileDialog
                {
                    IsFolderPicker = true,
                    Title = "Choose a directory to save the image into"
                };

                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    screenshot.Directory = dialog.FileName;
                }
            }

        }
        private bool CanChangeScreenshotDirectory(object arg)
        {
            return true;
        }

        #endregion

        #region Delete Screenshot details Command 

        public DelegateCommand<object> DeleteScreenshotDetailsCommand { get; private set; }

        /// <summary>
        /// Deletes the screenshotDetails object from the database.
        /// </summary>
        /// <param name="screenshotDetailsId">ID of the screenshot details object</param>
        private async void DeleteScreenshotDetailsAsync(object screenshotDetails)
        {
            if (screenshotDetails is ScreenshotDetails screenshot)
            {
                string query = "DELETE FROM ScreenshotDetails WHERE " +
                            "ID = @ID";
                if (await DAL.ExecuteQueryAsync(query, screenshot))
                {
                    List.Remove(screenshot);
                }
            }
        }
        private bool CanDeleteScreenshotDetails(object arg)
        {
            return true;
        }

        #endregion

        #region New Screenshot Command

        public DelegateCommand NewScreenshotCommand { get; private set; }

        private async void AddNewScreenshot()
        {
            // get the main window state
            WindowState windowState = Application.Current.MainWindow.WindowState;
            // minimize the main window
            Application.Current.MainWindow.WindowState = WindowState.Minimized;

            NewScreenshotDetailsView view = new NewScreenshotDetailsView()
            {
                Owner = Application.Current.MainWindow
            };
            if (view.ShowDialog() == true)
            {
                string query = $"INSERT INTO ScreenshotDetails (Directory,TimeScheduled,ImageName,URL) VALUES (@Directory," +
                    $"@TimeScheduled,@ImageName,@URL);SELECT SEQ from sqlite_sequence WHERE name='ScreenshotDetails'";

                var newScreenshot = view.NewScreenshot;
                var paramter = new
                {
                    newScreenshot.Directory,
                    TimeScheduled = newScreenshot.TimeScheduled.ToShortTimeString(),
                    newScreenshot.ImageName,
                    newScreenshot.URL
                };

                newScreenshot.ID = await DAL.InsertNewScresnshot(query, paramter);

                List.Add(newScreenshot);
            }

            Application.Current.MainWindow.WindowState = windowState;
        }


        #endregion

        #endregion

        #region Event Handlers

        /// <summary>
        /// Invokes when an item inside <see cref="List"/> changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void List_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e?.ListChangedType == ListChangedType.ItemChanged)
            {
                var item = List[e.NewIndex];
                // get the proptery info for the changed property from the item.
                PropertyInfo prop = item?.GetType().GetProperty(e.PropertyDescriptor.Name,
                    BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanRead)
                {
                    var value = prop.GetValue(item);
                    if (prop.Name == nameof(item.TimeScheduled))
                    {
                        value = Convert.ToDateTime(prop.GetValue(item)).ToLocalTime().ToShortTimeString();
                    }
                    Console.WriteLine(value);

                    string query = $"Update {nameof(ScreenshotDetails)} set {prop.Name} = @Value " +
                        $"Where {nameof(item.ID)} = @{nameof(item.ID)}";

                    if (!DAL.ExecuteQueryAsync(query, new { Value = value, item.ID }).Result)
                    {
                        Helper.MessageBox.ShowMessageBox($"An unknown error occurred while updating the {prop.Name}.",
                        $"Unable to update");
                        return;
                    }

                }
            }
        }

        #endregion

    }
}
