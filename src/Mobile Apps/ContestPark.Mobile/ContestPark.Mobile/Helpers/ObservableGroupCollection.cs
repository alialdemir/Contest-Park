using ContestPark.Mobile.Models;
using ContestPark.Mobile.Models.Base;
using System.Collections.ObjectModel;
using System.Linq;

namespace ContestPark.Mobile.Helpers
{
    public class ObservableGroupCollection<S, T> : ObservableCollection<T>, IModelBase
    {
        private readonly S _key;

        public ObservableGroupCollection(IGrouping<S, T> group)
            : base(group)
        {
            _key = group.Key;
        }

        public S Key
        {
            get { return _key; }
        }
    }
}
