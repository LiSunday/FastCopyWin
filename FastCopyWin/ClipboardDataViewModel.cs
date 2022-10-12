using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FastCopyWin
{
    public class ClipboardDataViewModel
    {
        private ObservableCollection<Object> dataList = new();

        private ICollectionView? _collectionView;
        internal ICollectionView CollectionView
        {
            get
            {
                _collectionView ??= CollectionViewSource.GetDefaultView(dataList);
                return _collectionView;
            }
        }

        internal void AddFirstData(object data)
        {
            dataList.Insert(0, data);
        }
    }
}