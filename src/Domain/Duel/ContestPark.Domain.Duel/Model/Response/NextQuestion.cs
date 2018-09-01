using ContestPark.Domain.Duel.Enums;
using Newtonsoft.Json;
using System;

namespace ContestPark.Domain.Duel.Model.Response
{
    public class NextQuestion
    {
        public Domain.Question.Model.Response.Question Question { get; set; }

        public Stylish FounderStylish { get; set; }

        public Stylish OpponentStylish { get; set; }

        public byte FounderScore { get; set; }

        public byte OpponentScore { get; set; }

        public Guid Id { get; set; }

        [JsonIgnore]
        public int DuelId { get; set; }
        public bool IsGameEnd { get; set; }
    }
}