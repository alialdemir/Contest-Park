using ContestPark.Core.Dapper;
using ContestPark.Core.Domain;
using ContestPark.Core.Domain.Model;
using ContestPark.Core.Enums;
using ContestPark.Core.Extensions;
using ContestPark.Core.Interfaces;
using ContestPark.Domain.Category.Model.Response;
using ContestPark.Infrastructure.Category.Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

using model = ContestPark.Domain.Category.Model.Response;

namespace ContestPark.Infrastructure.Category.Repositories.Category
{
    internal class CategoryRepository : DapperRepositoryBase<CategoryEntity>, ICategoryRepository
    {
        #region Constructor

        public CategoryRepository(ISettingsBase settingsBase) : base(settingsBase)
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kategorilerin listesi
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Kategori listesi</returns>
        public ServiceResponse<model.Category> GetCategoryList(string userId, Languages language, Paging paging)
        {
            string sql = @"SELECT ( SELECT TOP(1) [p].[CategoryName] FROM [CategoryLangs] AS [p] WHERE ([p].[LanguageId] = @LangId) AND ([sc].[CategoryId] = [p].[CategoryId]) ) AS [CategoryName],
						   [sc].[CategoryId] as [CategoryId],
						   [cc].[Color] as [Color],
						   [sc].[SubCategoryId],
						   [sc].[DisplayPrice],
						   (SELECT TOP(1) [scl].[SubCategoryName] FROM [SubCategoryLangs] AS [scl] WHERE ([scl].[LanguageId] = @LangId) AND ([sc].[SubCategoryId] = [scl].[SubCategoryId])) AS [SubCategoryName],
						   (case (SELECT
						   (CASE
						   WHEN EXISTS(
						   SELECT NULL AS [EMPTY]
						   FROM OpenSubCategories AS osc  where osc.UserId =@UserId and osc.SubCategoryId = sc.SubCategoryId
						   ) THEN 1
						   ELSE 0
						   END) )
						   when 1 then 0
						   else sc.Price
						   end) as Price,
						   (case
						   when [sc].[Price] = 0 then [sc].[PictuePath]
						   when (SELECT
						   (CASE
						   WHEN EXISTS(
						   SELECT NULL AS [EMPTY]
						   FROM [OpenSubCategories] AS [osc]  where [osc].[UserId] =@UserId and [osc].[SubCategoryId] = [sc].[SubCategoryId]
						   ) THEN 1
						   ELSE 0
						   END) ) = 1 then [sc].[PictuePath]
						   else @PicturePath
						   end) as PicturePath
						   FROM [Categories] AS [cc]
						   INNER JOIN [SubCategories] AS [sc] ON [cc].[CategoryId] = [sc].[CategoryId]
						   WHERE ([cc].[Visibility] = 1) AND ([sc].[Visibility] = 1)
						   ORDER BY [cc].[Order]",
                           offset = "OFFSET @PageNumber ROWS FETCH NEXT @PageSize ROWS ONLY";

            var param = new
            {
                UserId = userId,
                LangId = (int)language,
                PicturePath = DefaultImages.DefaultLock,
                PageNumber = QueryableExtension.PageNumberCalculate(paging),
                paging.PageSize
            };

            var lookup = new Dictionary<int, Domain.Category.Model.Response.Category>();
            Connection.Query<model.Category, model.SubCategory, model.Category>(sql + offset, (category, subCategory) =>
            {
                if (!lookup.TryGetValue(category.CategoryId, out model.Category invoiceEntry))
                {
                    invoiceEntry = category;
                    if (invoiceEntry.SubCategories == null) invoiceEntry.SubCategories = new List<SubCategory>();
                    lookup.Add(invoiceEntry.CategoryId, invoiceEntry);
                }
                invoiceEntry.SubCategories.Add(subCategory);
                return invoiceEntry;
            }, param, splitOn: "ContestCategoryId,SubCategoryId").ToList();

            return new ServiceResponse<model.Category>
            {
                Items = lookup.Values.Distinct(),
                PageNumber = paging.PageNumber,
                PageSize = paging.PageSize,
                Count = Connection.Query<int>($"SELECT COUNT(*) FROM ({ "SELECT TOP (100) PERCENT " + sql.Substring(6)}) AS c;", param).First()
            };
        }

        /// <summary>
        /// Alt kategori Id'ye göre kategori listesi getirir
        /// </summary>
        /// <param name="userId">Kullanıcı Id</param>
        /// <param name="categoryId">Alt kategori Id</param>
        /// <param name="language">Kullanıcı dili</param>
        /// <param name="paging">Sayfalama</param>
        /// <returns>Aranan kategorilerin listesi</returns>
        public ServiceResponse<SubCategorySearch> CategorySearch(string userId, Int16 categoryId, Languages language, Paging paging)
        {
            string sql = @"SELECT
						   [cl].[CategoryName],
						   [scl].[SubCategoryName],
						   [c].[Color] as [Color],
						   [sc].[SubCategoryId],
						   [sc].[DisplayPrice],
						   (case (SELECT
						   (CASE
						   WHEN EXISTS(
						   SELECT NULL AS [EMPTY]
						   FROM OpenSubCategories AS osc  where osc.UserId =@userId and osc.SubCategoryId = sc.SubCategoryId
						   ) THEN 1
						   ELSE 0
						   END) )
						   when 1 then 0
						   else sc.Price
						   end) as Price,
						   (case
						   when [sc].[Price] = 0 then [sc].[PictuePath]
						   when (SELECT
						   (CASE
						   WHEN EXISTS(
						   SELECT NULL AS [EMPTY]
						   FROM [OpenSubCategories] AS [osc]  where [osc].[UserId] =@userId and [osc].[SubCategoryId] = [sc].[SubCategoryId]
						   ) THEN 1
						   ELSE 0
						   END) ) = 1 then [sc].[PictuePath]
						   else @picturePath
						   end) as PicturePath
						   FROM [SubCategories] AS [sc]
						   INNER JOIN  [Categories] AS [c] on [sc].[CategoryId]=[c].[CategoryId]
						   INNER JOIN [CategoryLangs] AS [cl] on [c].[CategoryId]=[cl].[CategoryId]
						   INNER JOIN [SubCategoryLangs] AS [scl] on [sc].SubCategoryId=[scl].[SubCategoryId]
						   WHERE [cl].[LanguageId]=@language AND [scl].[LanguageId]=@language AND [c].[Visibility] = 1 AND [sc].[Visibility] = 1 AND [c].[CategoryId]=@categoryId
						   ORDER BY [sc].[Price]";

            return Connection.QueryPaging<SubCategorySearch>(sql, new
            {
                userId,
                categoryId,
                picturePath = DefaultImages.DefaultLock,
                language
            }, paging);
        }

        #endregion Methods
    }
}