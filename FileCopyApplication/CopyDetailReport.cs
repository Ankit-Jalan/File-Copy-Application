
namespace FileCopyApplication
{
    /// <summary>
    /// Class contains copy details like id, File name , Pass or Fail, Reason of Failer.
    /// </summary>
    public class CopyDetailReport
    {
        /// <summary>
        /// Contains Id for each file.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Contains File Name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Contains Pass if copy is successfully completed otherwise fail.
        /// </summary>
        public string PassInfo { get; set; }

        /// <summary>
        /// If copy of file fails than it contain reason of fail otherwise it contains empty string.
        /// </summary>
        public string Reason { get; set; }
    }
}
