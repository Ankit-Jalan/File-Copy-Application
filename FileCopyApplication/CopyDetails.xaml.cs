using System.Windows;
using System.Collections.ObjectModel;

namespace FileCopyApplication
{
    /// <summary>
    /// Interaction logic for CopyDetails.xaml
    /// </summary>
    public partial class CopyDetails : Window
    {
        public CopyDetails()
        {}

        /// <summary>
        /// Initialize components and set observableCollection to datagrid.
        /// </summary>
        /// <param name="copyDetailsList"></param>
        public CopyDetails(ObservableCollection<CopyDetailReport> copyDetailsList)
        {
            InitializeComponent();
            copyDetailDataGrid.ItemsSource = copyDetailsList;
        }
    }
}
