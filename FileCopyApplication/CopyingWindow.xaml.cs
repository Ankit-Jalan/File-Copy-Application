using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;
namespace FileCopyApplication
{
    /// <summary>
    /// Interaction logic for CopyingWindow.xaml
    /// </summary>
    public partial class CopyingWindow : Window
    {
        #region Fields
        /// <summary>
        /// This list contains object of SourcePathSetter class
        /// and used to copy all source path files.
        /// </summary>
        ObservableCollection<SourcePathSetter> sourceObjectList;

        /// <summary>
        /// BackgroundWorker is used to copy in background 
        /// and reporting progress to screen.
        /// </summary>
        BackgroundWorker backgroundCopy;

        /// <summary>
        /// This string provied Destination directory 
        /// where all the files are copied
        /// </summary>
        string destinationPath;

        /// <summary>
        /// Notify that overwrite of file is allowed or not.
        /// </summary>
        bool isOverwrite;

        /// <summary>
        /// Contains total size of copying data.
        /// </summary>
        long totalSize = 0;
             
        /// <summary>
        /// Contains Size of copied data
        /// </summary>
        double copied;
        
        /// <summary>
        /// Stopwatch is used to get time 
        /// which is elapsed during copying.
        /// </summary>
        Stopwatch copytime;
       
        /// <summary>
        /// Contains progress percentage value.
        /// </summary>
        double percentage;

        /// <summary>
        /// This list contains object of CopyDetail class
        /// and used to copy all source path files.
        /// </summary>
        ObservableCollection<CopyDetailReport> copyDetailObject;        

        #endregion

        public CopyingWindow()
        {}

        /// <summary>
        /// Copying Window constructor is Parameterize constructor.
        /// </summary>
        /// <param name="sourceList">SourcePathSetter object List</param>
        /// <param name="toPath">Destination Directory Path</param>
        /// <param name="overWrite">Overwrite Permission</param>
        public CopyingWindow(ObservableCollection<SourcePathSetter> sourceList , string toPath,bool overWrite)
        {
            InitializeComponent();
            sourceObjectList = sourceList;
            destinationPath = toPath;
            isOverwrite = overWrite;
            copytime = new Stopwatch();
            Closing += new CancelEventHandler(Confirm_Closing);
            copyDetailObject = new ObservableCollection<CopyDetailReport>();
            backgroundCopy = new BackgroundWorker();
            backgroundCopy.WorkerReportsProgress = true;
            backgroundCopy.WorkerSupportsCancellation = true;
            backgroundCopy.DoWork += backgroundCopy_DoWork;
            backgroundCopy.ProgressChanged += backgroundCopy_ProgressChanged;
            backgroundCopy.RunWorkerCompleted += backgroundCopy_RunWorkerCompleted;
            StartCopy();
        }

        #region Events
        
        /// <summary>
        /// This event is copying data to destination directory 
        /// and updating progress on screen;
        /// </summary>
        /// <param name="sender">BackgroundWorker Object</param>
        /// <param name="e"> DoWorkerEventArgs Object</param>
        void backgroundCopy_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            long totalCopied = 0;                      
            foreach (SourcePathSetter sourceObject in sourceObjectList)
            {
                string passinfo = ConstValues.Pass;
                string reason = String.Empty;                
                
                if (File.Exists(sourceObject.SourcePath))
                {
                    try
                    {
                        if (totalSize == 0)
                        {
                            foreach (SourcePathSetter sourceForSize in sourceObjectList)
                            {
                                if (File.Exists(sourceForSize.SourcePath))
                                {
                                    totalSize = totalSize + new FileInfo(sourceForSize.SourcePath).Length;
                                }
                            }
                        }
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            fileName.Text = Path.GetFileName(sourceObject.SourcePath);
                        }));
                        bool filePresent = File.Exists(Path.Combine(destinationPath, Path.GetFileName(sourceObject.SourcePath)));
                        if (filePresent && isOverwrite == false && totalSize != 0)
                        {
                            passinfo = ConstValues.Fail;
                            reason = ConstValues.OverwiteFail;                            
                        }
                        else if (totalSize != 0)
                        {
                            if (filePresent)
                            {
                                File.Delete(Path.Combine(destinationPath, Path.GetFileName(sourceObject.SourcePath)));
                            }

                            int bufferSize = ConstValues.BufferSize;
                            using (FileStream fin = new FileStream(sourceObject.SourcePath, FileMode.Open))
                            {
                                FileStream fout = new FileStream(Path.Combine(destinationPath, Path.GetFileName(sourceObject.SourcePath)), FileMode.Create);
                                int bytesRead = -1;
                                byte[] bytes = new byte[bufferSize];
                                while ((bytesRead = fin.Read(bytes, 0, bufferSize)) > 0)
                                {
                                    fout.Write(bytes, 0, bytesRead);
                                    long j = new FileInfo(fout.Name).Length;
                                    copied = totalCopied + j;
                                    percentage = copied / totalSize;
                                    worker.ReportProgress((int)(percentage * 100));
                                }
                            }                            
                        }
                        else
                        {
                            passinfo = ConstValues.Fail;
                            reason = ConstValues.CopySizeZero;
                            worker.ReportProgress(100);
                        }
                    }
                    catch (PathTooLongException)
                    {
                        passinfo = ConstValues.Fail;
                        reason = ConstValues.PathtooLong; 
                    }
                    catch (DirectoryNotFoundException)
                    {
                        passinfo = ConstValues.Fail;
                        reason = ConstValues.DirectoryNotFound;                        
                    }
                    catch (UnauthorizedAccessException)
                    {
                        passinfo = ConstValues.Fail;
                        reason = ConstValues.AcessRestricted;                        
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        passinfo = ConstValues.Fail;
                        reason = ConstValues.ModeIncorrect;                        
                    }
                    catch (FileNotFoundException)
                    {
                        passinfo = ConstValues.Fail;
                        reason = ConstValues.FileNotPresent;
                    }
                    catch (IOException)
                    {
                        passinfo = ConstValues.Fail;
                        reason = ConstValues.FileInUse;                        
                    }
                    if (totalSize != 0)
                    {
                        totalCopied = totalCopied + new FileInfo(sourceObject.SourcePath).Length;
                        percentage = totalCopied / totalSize;
                        worker.ReportProgress((int)(percentage * 100));
                    }
                }
                else
                {
                    passinfo = ConstValues.Fail;
                    reason = ConstValues.FileNotPresent;                    
                }
                copyDetailObject.Add(new CopyDetailReport() { Id = copyDetailObject.Count + 1, FileName = sourceObject.SourcePath, PassInfo = passinfo, Reason = reason });
            }
            copytime.Stop();            
        }        

        /// <summary>
        /// This event update progress on progresbar and textbox.
        /// </summary>
        /// <param name="sender">BackgroundWorker Object</param>
        /// <param name="e">ProgressChangedEventArgs Object</param>
        void backgroundCopy_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            copyProgressBar.Value = e.ProgressPercentage;
            progressText.Text = e.ProgressPercentage.ToString() + ConstValues.Percentage;
            elapsedTime.Text = string.Format(ConstValues.FormatTime, copytime.Elapsed.TotalHours, copytime.Elapsed.Minutes, copytime.Elapsed.Seconds);
            if (copied > 0 && totalSize > 0)
            {
                double remainingTicks = ((double)(totalSize - copied) * ((double)(copytime.Elapsed.Ticks / copied)));
                TimeSpan remaining = new TimeSpan((long)remainingTicks);
                remainingTime.Text = string.Format(ConstValues.FormatTime, remaining.TotalHours, remaining.Minutes, remaining.Seconds);                
            }
            else
            {
                remainingTime.Text = ConstValues.RemainingTimeonStart;
            }                
        }

        /// <summary>
        /// This event invoke copyDetailDialog and passes copy details.
        /// </summary>
        /// <param name="sender">BackgroundWorker Object</param>
        /// <param name="e">RunWokerCompletedEventArgs Object</param>
        void backgroundCopy_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker backgroundObject = sender as BackgroundWorker;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }            
            else if (this.IsActive && !e.Cancelled)
            {
                CopyDetails showCopyDetails = new CopyDetails(copyDetailObject);
                this.Close();
                showCopyDetails.ShowDialog();
            }            
        }

        /// <summary>
        /// When data is copying  than it confirm before closing window.
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">CancelEventArgs object</param>
        void Confirm_Closing(object sender, CancelEventArgs e)
        {
            if (backgroundCopy.IsBusy)
            {
                MessageBoxResult result = MessageBox.Show(ConstValues.ClosingMessage, ConstValues.ConfirmClose, MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else
                {                    
                    copytime.Stop();
                    backgroundCopy.CancelAsync();
                }
            }
        }
        #endregion

        /// <summary>        
        /// Start background copying and start stopwatch.
        /// </summary>
        private void StartCopy()
        {
            if (!backgroundCopy.IsBusy)
            {
                copytime.Start();
                backgroundCopy.RunWorkerAsync();
            }
        }        
    }
}
