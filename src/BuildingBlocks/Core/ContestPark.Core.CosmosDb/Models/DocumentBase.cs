﻿using ContestPark.Core.Database.Models;
using Newtonsoft.Json;
using System;

namespace ContestPark.Core.CosmosDb.Models
{
    public abstract class DocumentBase : EntityBase
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
