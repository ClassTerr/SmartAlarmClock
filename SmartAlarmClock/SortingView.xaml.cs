using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SortingEvolution3
{
    /// <summary>
    /// Тот же ListView, но с реализацией сортировки
    /// </summary>
    public partial class SortingView : UserControl
    {
        public SortingView()
        {
            InitializeComponent();
            DataContext = new SortingViewModel();
        }

        public ObservableCollection<object> Items
        {
            get
            {
                return (DataContext as SortingViewModel).ThirdResultData;
            }
            set
            {
                DataContext = value;
            }
        }

        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;

        private void ThirdResultDataViewClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = e.OriginalSource as GridViewColumnHeader;
            if (column == null)
            {
                return;
            }

            if (_sortColumn == column)
            {
                // Toggle sorting direction
                _sortDirection = _sortDirection == ListSortDirection.Ascending ?
                                                   ListSortDirection.Descending :
                                                   ListSortDirection.Ascending;
            }
            else
            {
                // Remove arrow from previously sorted header
                if (_sortColumn != null)
                {
                    _sortColumn.Column.HeaderTemplate = null;
                    _sortColumn.Column.Width = _sortColumn.ActualWidth - 20;
                }

                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
                column.Column.Width = column.ActualWidth + 20;
            }

            if (_sortDirection == ListSortDirection.Ascending)
            {
                column.Column.HeaderTemplate =
                                   Resources["ArrowUp"] as DataTemplate;
            }
            else
            {
                column.Column.HeaderTemplate =
                                    Resources["ArrowDown"] as DataTemplate;
            }

            string header = string.Empty;

            // if binding is used and property name doesn't match header content
            Binding b = _sortColumn.Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
            }

            var viewModel = DataContext as SortingViewModel;
            viewModel.Sort(header);
        }
    }
}
