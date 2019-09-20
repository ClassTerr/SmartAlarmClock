using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace SortingEvolution3
{
    /// <summary>
    /// Служебный класс, реализующий сортировку в ListView
    /// </summary>
    public class SortingViewModel
    {
        public SortingViewModel()
        {
            ThirdResultData = new ObservableCollection<object>();
        }

        private ObservableCollection<object> _thirdResultData;
        private CollectionViewSource _thirdResultDataView;
        private string _sortColumn;
        private ListSortDirection _sortDirection;

        public ObservableCollection<object> ThirdResultData
        {
            get
            {
                return _thirdResultData;
            }
            set
            {
                _thirdResultData = value;
                _thirdResultDataView = new CollectionViewSource();
                _thirdResultDataView.Source = _thirdResultData;
            }
        }

        public ListCollectionView ThirdResultDataView
        {
            get
            {
                return (ListCollectionView)_thirdResultDataView.View;
            }
        }

        public void Sort(string column)
        {
            if (_sortColumn == column)
            {
                // Toggle sorting direction
                _sortDirection = _sortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
            }

            _thirdResultDataView.SortDescriptions.Clear();
            _thirdResultDataView.SortDescriptions.Add(new SortDescription(_sortColumn, _sortDirection));
        }
    }
}
