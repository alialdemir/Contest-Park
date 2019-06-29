﻿using ContestPark.Core.CosmosDb.Models;
using ContestPark.Core.Models;

namespace ContestPark.Post.API.Infrastructure.Documents
{
    /// <summary>
    /// Bu dökümentin id değeri kullanıcı id
    /// </summary>
    public class User : DocumentBase
    {
        public string FullName { get; set; }

        public string UserName { get; set; }
        private string _profilePicturePath;

        public string ProfilePicturePath
        {
            get { return !string.IsNullOrEmpty(_profilePicturePath) ? _profilePicturePath : DefaultImages.DefaultProfilePicture; }
            set { _profilePicturePath = value; }
        }

        /// <summary>
        /// Takip ettiklerim
        /// </summary>
        public long FollowingCount { get; set; }

        /// <summary>
        /// Takip edenler
        /// </summary>
        public long FollowersCount { get; set; }
    }
}
