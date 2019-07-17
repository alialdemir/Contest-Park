using ContestPark.Core.Database.Models;
using System;

namespace ContestPark.Duel.API.Models
{
    public class RankingModel
    {
        public ServiceModel<RankModel> Ranks { get; set; }
        public DateTime ContestFinishDate { get; set; }
    }
}
