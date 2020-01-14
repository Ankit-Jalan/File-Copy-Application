using System;
using System.ComponentModel;

namespace FileCopyApplication
{
    /// <summary>
    /// Class contains propety source path.
    /// </summary>
    public class SourcePathSetter : INotifyPropertyChanged
    {
        private String sourcePath;
        /// <summary>
        /// Contains source path.
        /// </summary>
        public String SourcePath 
        {
            get
            {
                return this.sourcePath;
            }
            set
            {
                if (this.sourcePath != value)
                {
                    this.sourcePath = value;
                    this.NotifyPropertyChanged("SourcePath");
                }
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propSourcePath)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propSourcePath));
        }
    }
}
