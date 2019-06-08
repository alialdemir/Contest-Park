using ContestPark.Core.CosmosDb.Models;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace ContestPark.Core.CosmosDb.Extensions
{
    public static class QueryableExtension
    {
        public static long PageNumberCalculate(PagingModel paging)
        {
            return paging.PageSize * (paging.PageNumber - 1);
        }
    }
}