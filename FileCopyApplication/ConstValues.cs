
namespace FileCopyApplication
{
    /// <summary>
    /// class contains constant values that is used in CopyApplication.
    /// </summary>
    class ConstValues
    {        
        internal const string FileNotExist = "{0} file does not exists.";
        internal const string Percentage = "%";
        internal const string FormatTime = "{0:00}:{1:00}:{2:00}";        
        internal const string DirectoryNotExits = "{0} directory does not exists.";
        internal const string InvalidSource = " Please enter valid File Path";
        internal const string InvalidDestination = "Please enter valid Directory Path";
        internal const string ClosingMessage = " Do you want to cancel copying file";
        internal const string ConfirmClose = "Confirm Close";
        internal const string DuplicateFile = "Duplicate file name is not allowed";
        internal const string SourcePathEmpty = "Source Path row is blank, please fill the row or remove the row";
        internal const string Pass = "Pass";
        internal const string Fail = "Fail";
        internal const string PathtooLong = "Path Too Long";
        internal const string DirectoryNotFound = "Directory not found";
        internal const string InsufficientSpace = "Drive space is insufficient";
        internal const string FileNotPresent = "File Not Present";
        internal const string YesRadioButton = "yesRadioBtn";
        internal const string FileInUse = "File is being used by some Process";        
        internal const string NOReadPermission = "{0}\nYou don't have permission to Open this file";
        internal const string AcessRestricted = "You can not have acess permission.";
        internal const string ModeIncorrect = "Mode Incorrect";                
        internal const string InCorrectInput = "Incorrect input";
        internal const string FilePathEmpty = "File Path is Empty.";
        internal const string OverwiteFail = "File with same name  already Present.";        
        internal const string RemainingTimeonStart = "Calculating...";
        internal const string SameDirectory = "You can not copy file on same directory";
        internal const int BufferSize = 1024*1024;
        internal const string OnlyFilePath = "Only File path can be accepted";
        internal const string CopySizeZero = "Copy Size is Zero";
    }
}
