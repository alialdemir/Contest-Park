using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Ranking;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Score
{
    public class ScoreMockService : IScoreService
    {
        public Task<ServiceModel<RankingModel>> FollowingRankingAsync(short subCategoryId, PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<RankingModel>
            {
                Items = new List<RankingModel>
                {
                         new RankingModel
                    {
                        TotalScore = 1000000,
                        UserFullName = "Ali Aldemir",
                        UserName = "witcherfearless",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 80000,
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 76345,
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    }
                }
            });
        }

        public Task<TimeLeftModel> GetTimeLeft(short subCategoryId)
        {
            DateTime endDate = DateTime.Now.AddMonths(1);

            TimeSpan diff = endDate - DateTime.Now;

            System.Collections.Generic.Dictionary<int, string> months = new Dictionary<int, string>
            {
                {1, "Jan" },
                {2, "Feb" },
                {3, "Mar" },
                {4, "Apr" },
                {5, "May" },
                {6, "Jun" },
                {7, "Jul" },
                {8, "Aug" },
                {9, "Sep" },
                {10, "Oct" },
                {11, "Now" },
                {12, "Dec" },
            };

            return Task.FromResult(new TimeLeftModel
            {
                Months = months[DateTime.Now.Month],
                FinsihDate = DateTime.Now.AddMonths(1),
                TimeLeft = diff.Days + " d " + diff.Hours + " h " + diff.Minutes + " m  " + diff.Seconds + " s"
            });
        }

        public Task<ServiceModel<RankingModel>> SubCategoryRankingAsync(short subCategoryId, PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<RankingModel>
            {
                Items = new List<RankingModel>
                {
                         new RankingModel
                    {
                        TotalScore = 1000000,
                        UserFullName = "Ali Aldemir",
                        UserName = "witcherfearless",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 80000,
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 76345,
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 80000,
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 76345,
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 80000,
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 76345,
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 80000,
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 76345,
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 80000,
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = 76345,
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    }
                }
            });
        }
    }
}