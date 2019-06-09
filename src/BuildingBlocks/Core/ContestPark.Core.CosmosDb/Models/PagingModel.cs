using Newtonsoft.Json;

namespace ContestPark.Core.CosmosDb.Models
{
    public class PagingModel
    {
        /// <summary>
        /// Sayfa başına gösterilen kayıt sayısı default 10
        /// </summary>
        private int _pageSize = 10;

        public int PageSize
        {
            get { return _pageSize; }
            set { if (value > 0) _pageSize = value; }
        }

        /// <summary>
        /// Sayfa numarası
        /// </summary>
        private int _pageNumber = 1;

        public int PageNumber
        {
            get { return _pageNumber; }
            set { if (value > 0) _pageNumber = value; }
        }

        public override string ToString()
        {
            return $"?{nameof(PageSize)}={PageSize}&{nameof(PageNumber)}={PageNumber}";
        }

        [JsonIgnore]
        public int Offset
        {
            get
            {
                if (PageNumber == 1)
                    return PageSize * (PageNumber - 1);

                return (PageSize * (PageNumber - 1));
            }
        }
    }
}