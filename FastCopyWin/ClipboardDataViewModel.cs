using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
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
            if (dataList.Count == 10) dataList.RemoveAt(dataList.Count - 1);
            dataList.Insert(0, data);
        }

        internal void AddUnlikeFirstData(object data)
        {
            lock (dataList) {
                if (dataList.Count != 0 && data.Equals(dataList[0])) return;
                AddFirstData(data);
            }
        }
    }
}