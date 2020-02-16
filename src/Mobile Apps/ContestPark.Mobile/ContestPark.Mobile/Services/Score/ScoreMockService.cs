using ContestPark.Mobile.Enums;
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
        public Task<RankModel> AllTimesAsync(PagingModel pagingModel)
        {
            return SubCategoryRankingAsync(1, BalanceTypes.Money, new PagingModel());
        }

        public Task<RankModel> FollowingRankingAsync(short subCategoryId, BalanceTypes balanceType, PagingModel pagingModel)
        {
            return Task.FromResult(
                new RankModel
                {
                    ContestFinishDate = DateTime.Now.AddDays(35),
                    Ranks = new ServiceModel<RankingModel>
                    {
                        Items = new List<RankingModel>
                {
                         new RankingModel
                    {
                        TotalScore = "1000k",
                        UserFullName = "Ali Aldemir",
                        UserName = "witcherfearless",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "1k",
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "65",
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    }
                }
                    }
                });
        }

        public Task<RankModel> SubCategoryRankingAsync(short subCategoryId, BalanceTypes balanceType, PagingModel pagingModel)
        {
            return Task.FromResult(

                new RankModel
                {
                    ContestFinishDate = DateTime.Now.AddDays(35),
                    Ranks = new ServiceModel<RankingModel>
                    {
                        Items = new List<RankingModel>
                {
                         new RankingModel
                    {
                        TotalScore = "10k",
                        UserFullName = "Ali Aldemir",
                        UserName = "witcherfearless",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "8k",
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "7.6k",
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "3k",
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "900",
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "760",
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "350",
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "200",
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "150",
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "120",
                        UserFullName = "Melike Sal",
                        UserName = "melike",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    },
                          new RankingModel
                    {
                        TotalScore = "70",
                        UserFullName = "Zehra Paltik",
                        UserName = "zehra",
                        UserProfilePicturePath = DefaultImages.DefaultProfilePicture
                    }
                }
                    }
                });
        }
    }
}
