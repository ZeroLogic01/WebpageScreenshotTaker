using System;
using System.ComponentModel;

namespace WebpageScreenshotTaker.Data.Models
{
    public class ScreenshotDetails : INotifyPropertyChanged
    {
        #region INotifyProperty Changed Event Handler

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #endregion

        #region Private Fields

        private string _directory;
        private DateTime _timeScheduled = default;
        private string _imageName;
        private string _url;

        #endregion

        #region Public Properties

        /// <summary>
        /// The screenshot ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Directory to save the files into. 
        /// </summary>
        public string Directory
        {
            get { return _directory; }
            set
            {
                value = value.Trim();
                if (value == _directory || string.IsNullOrWhiteSpace(value))
                    return;

                _directory = value;

                // Raise the property changed, because the path will be changed through the button,
                // so we have to notify the text box control as well as the
                // ViewModel's BindingList about this change.
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Directory)));
            }
        }

        /// <summary>
        /// Time of day to take the image. 
        /// </summary>
        public DateTime TimeScheduled
        {
            get { return _timeScheduled; }
            set
            {
                if (value == _timeScheduled)
                    return;

                _timeScheduled = value;

                // Raise the property changed to notify the ViewModel's BindingList about this change
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeScheduled)));

            }
        }

        /// <summary>
        /// Name of the image. 
        /// </summary>
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                value = value.Trim();
                if (value == _imageName || string.IsNullOrWhiteSpace(value))
                    return;

                _imageName = value;
                // Raise the property changed to notify the ViewModel's BindingList about this change
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ImageName)));

            }
        }

        /// <summary>
        /// URL of the image. 
        /// </summary>
        public string URL
        {
            get { return _url; }
            set
            {
                value = value.Trim();
                if (value == _url || string.IsNullOrWhiteSpace(value))
                    return;

                _url = value;
                // Raise the property changed so that the ViewModel's BindingList gets notified.
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(URL)));

            }
        }


        #endregion

    }
}
