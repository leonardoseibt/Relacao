using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Relacao.Classes
{
    class ObsCollection<T> : ObservableCollection<T>
    {
        public void UpdateCollection()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Reset));
        }
    }
}
