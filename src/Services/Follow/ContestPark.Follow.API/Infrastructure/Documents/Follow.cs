using ContestPark.Core.CosmosDb.Models;
using System.Collections.Generic;
using System.Linq;

namespace ContestPark.Follow.API.Infrastructure.Documents
{
    /// <summary>
    /// Bu dökümentin id değeri kullanıcı id
    /// </summary>
    public class Follow : DocumentBase
    {
        /// <summary>
        /// Kullanıcının takip ettikleri
        /// </summary>
        public List<string> Following { get; set; } = new List<string>();

        /// <summary>
        /// Kullanıcıyı takip edenler
        /// </summary>
        public List<string> Followers { get; set; } = new List<string>();

        public long FollowingCount
        {
            get { return Following.Count(); }
        }

        public long FollowersCount
        {
            get { return Followers.Count(); }
        }
    }
}