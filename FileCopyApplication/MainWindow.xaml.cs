using System.Windows;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Controls;
using System.IO;
using System;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Linq;
namespace FileCopyApplication
{    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        /// <summary>
        /// This list contains object of SourcePathSetter class
        /// and used to set and get value of copy source path.
        /// </summary>
        ObservableCollection<SourcePathSetter> sourcePathList;        
        #endregion
        
        public MainWindow()
        {
            InitializeComponent();
            sourcePathList = new ObservableCollection<SourcePathSetter>();
            sourcePathList.Insert(sourcePathList.Count, new SourcePathSetter());            
            sourceDataGrid.ItemsSource = sourcePathList;            
        }

        #region Events

        /// <summary>
        /// This event is used to show copying window and 
        /// passes required parameter for copy. 
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">RoutedEventArgs object</param>
        public void copy_Click(object sender, RoutedEventArgs e)
        {
            if (sourcePathList.All(s => String.IsNullOrWhiteSpace(s.SourcePath)))
            {                
                MessageBox.Show(ConstValues.InvalidSource);               
            }
            else if (sourcePathList.Any(s => String.IsNullOrWhiteSpace(s.SourcePath)))
            {
                MessageBox.Show(ConstValues.SourcePathEmpty); 
            }
            else if (!Directory.Exists(destinationPath.Text))
            {
                MessageBox.Show(ConstValues.InvalidDestination);
            }
            else if (sourcePathList.Any(s => Path.GetDirectoryName(s.SourcePath) == destinationPath.Text))
            {
                MessageBox.Show(ConstValues.SameDirectory);
            }
            else 
            {
                CopyingWindow copyWindow = new CopyingWindow(sourcePathList, destinationPath.Text, (bool)yesRadioBtn.IsChecked);
                copyWindow.ShowDialog();
            }                                   
        }
         
        /// <summary>
        /// This event handle browsing of file to select
        /// copy file and set it to sourcePathList.
        /// </summary>
        /// <param name="sender">Button Object</param>
        /// <param name="e"> RoutedEventArgs object</param>
        public void browseSource_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();            
            if ((bool)openFileDialog.ShowDialog())
            {
                if (sourcePathList.Any(s => Path.GetFileName(s.SourcePath) == Path.GetFileName(openFileDialog.FileName) && s != (sourceDataGrid.SelectedItem as SourcePathSetter)))
                {
                    MessageBox.Show(ConstValues.DuplicateFile);
                }
                else
                {
                    (sourceDataGrid.SelectedItem as SourcePathSetter).SourcePath = openFileDialog.FileName;
                }
                    
            }
        }

        /// <summary>
        /// This event browse directory where we need to copy files.
        /// </summary>
        /// <param name="sender">Button Object</param>
        /// <param name="e"> RoutedEventArgs object</param>
        public void browseDestination_Click(object sender, RoutedEventArgs e)
        {            
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result =folderDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    destinationPath.Text = folderDialog.SelectedPath;
                }            
            }
        }

        /// <summary>
        /// This event adds new line to the last of list.
        /// </summary>
        /// <param name="sender">Button Object</param>
        /// <param name="e">RoutedEventArgs Object</param>
        public void addLine_Click(object sender, RoutedEventArgs e)
        {                       
            sourcePathList.Insert(sourcePathList.Count,new SourcePathSetter());
        }

        /// <summary>
        /// This event remove line for which user presses minus button.
        /// </summary>
        /// <param name="sender">Button Object</param>
        /// <param name="e">RoutedEventArgs Object</param>
        public void removeLine_Click(object sender, RoutedEventArgs e)
        {
            if (sourcePathList.Count > 1)
            {
                sourcePathList.Remove(sourceDataGrid.SelectedItem as SourcePathSetter);
            }
            else if (sourcePathList.Count == 1)
            {
                (sourceDataGrid.SelectedItem as SourcePathSetter).SourcePath = String.Empty;
            }
        }

        /// <summary>
        /// This event is checking that the file
        /// which is entered manual is present or not and 
        /// also check that the file is already selected or not.
        /// </summary>
        /// <param name="sender">TextBox Object</param>
        /// <param name="e">RoutedEventArgs</param>
        public void checkFilePath_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox filePathTextObj = sender as TextBox;
           
            try
            {
                if (File.Exists(filePathTextObj.Text))
                {
                    if (!Directory.Exists(filePathTextObj.Text))
                    {
                        FileSecurity fileSecurityObject = new FileSecurity(filePathTextObj.Text, AccessControlSections.Access);
                        AuthorizationRuleCollection arcObject = fileSecurityObject.GetAccessRules(true, true, typeof(NTAccount));
                        foreach (FileSystemAccessRule acessRule in arcObject)
                        {
                            if (acessRule.AccessControlType == AccessControlType.Deny && acessRule.IdentityReference.ToString() == WindowsIdentity.GetCurrent().Name)
                            {
                                if (acessRule.FileSystemRights == FileSystemRights.ReadAndExecute || acessRule.FileSystemRights == FileSystemRights.Read || acessRule.FileSystemRights == FileSystemRights.FullControl || acessRule.FileSystemRights == FileSystemRights.Modify)
                                {                                    
                                    MessageBox.Show(String.Format(ConstValues.NOReadPermission,filePathTextObj.Text));
                                    filePathTextObj.Text = String.Empty;
                                    return;
                                }
                            }
                        }
                        if (sourcePathList.Where(s => Path.GetFileName(s.SourcePath) == Path.GetFileName(filePathTextObj.Text)).Count() > 1)
                        {
                            MessageBox.Show(ConstValues.DuplicateFile);
                            filePathTextObj.Text = String.Empty;
                        }
                    }
                    else
                    {
                        MessageBox.Show(ConstValues.OnlyFilePath);
                        filePathTextObj.Text = String.Empty;
                    }                                    
                }
                else if (!String.IsNullOrWhiteSpace(filePathTextObj.Text))
                {
                
                        if (!Directory.Exists(filePathTextObj.Text))
                        {
                            FileSecurity fileSecurityObject = new FileSecurity(filePathTextObj.Text, AccessControlSections.Access);
                            AuthorizationRuleCollection arcObject = fileSecurityObject.GetAccessRules(true, true, typeof(NTAccount));                        
                            foreach (FileSystemAccessRule acessRule in arcObject)
                            {
                                if (acessRule.AccessControlType == AccessControlType.Deny && acessRule.IdentityReference.ToString() == WindowsIdentity.GetCurrent().Name)
                                {
                                    if (acessRule.FileSystemRights == FileSystemRights.ReadAndExecute || acessRule.FileSystemRights == FileSystemRights.Read || acessRule.FileSystemRights == FileSystemRights.FullControl || acessRule.FileSystemRights == FileSystemRights.Modify)
                                    {
                                        MessageBox.Show(String.Format(ConstValues.NOReadPermission, filePathTextObj.Text));
                                        filePathTextObj.Text = String.Empty;
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(ConstValues.OnlyFilePath);
                            filePathTextObj.Text = String.Empty;
                        }                                
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(ConstValues.NOReadPermission);
                filePathTextObj.Text = String.Empty;
            }
            catch (PathTooLongException)
            {
                MessageBox.Show(ConstValues.PathtooLong);
                filePathTextObj.Text = String.Empty;
            }
            catch (ArgumentException)
            {
                filePathTextObj.Text = String.Empty;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show(String.Format(ConstValues.FileNotExist, filePathTextObj.Text));
                filePathTextObj.Text = String.Empty;
            }
            catch (SystemException)
            {
                MessageBox.Show(String.Format(ConstValues.InvalidSource));
                filePathTextObj.Text = String.Empty;
            }
        }

        /// <summary>
        /// This event is checking that the directory
        /// which is entered manual is present or not.
        /// </summary>
        /// <param name="sender">TextBox Object</param>
        /// <param name="e">RoutedEventArgs</param>
        public void checkDirectoryPath_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox directoryPathTextObj = sender as TextBox;            
            if (!String.IsNullOrWhiteSpace(directoryPathTextObj.Text) && !Directory.Exists(directoryPathTextObj.Text))
            {
                try
                {
                    DirectorySecurity directorySecurityObject = new DirectorySecurity(Path.GetDirectoryName(directoryPathTextObj.Text), AccessControlSections.Access);
                    AuthorizationRuleCollection arcObject = directorySecurityObject.GetAccessRules(true, true, typeof(NTAccount));
                    foreach (FileSystemAccessRule acessRule in arcObject)
                    {
                        if (acessRule.AccessControlType == AccessControlType.Deny && acessRule.IdentityReference.ToString() == WindowsIdentity.GetCurrent().Name)
                        {
                            if (acessRule.FileSystemRights == FileSystemRights.ReadAndExecute || acessRule.FileSystemRights == FileSystemRights.Read || acessRule.FileSystemRights == FileSystemRights.FullControl || acessRule.FileSystemRights == FileSystemRights.Modify)
                            {
                                directoryPathTextObj.Text = String.Empty;
                                MessageBox.Show(ConstValues.NOReadPermission);
                                return;
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {                    
                    MessageBox.Show(ConstValues.NOReadPermission);
                }
                catch (PathTooLongException)
                {                    
                    MessageBox.Show(ConstValues.PathtooLong);
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show(String.Format(ConstValues.DirectoryNotExits, directoryPathTextObj.Text));                    
                }                
                catch (ArgumentException)
                {
                    MessageBox.Show(ConstValues.InvalidDestination);                    
                }
                catch (SystemException)
                {
                    MessageBox.Show(ConstValues.InvalidDestination);                    
                }
                directoryPathTextObj.Text = String.Empty;
            }
        }        

        #endregion
    }
}
