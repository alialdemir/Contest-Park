﻿using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Core.CosmosDb.Interfaces;
using Microsoft.Azure.Documents;
using System.Linq;

namespace ContestPark.Category.API.Infrastructure.Repositories.OpenCategory
{
    public class OpenCategoryRepository : IOpenCategoryRepository
    {
        #region Constructor

        public OpenCategoryRepository(IDocumentDbRepository<OpenSubCategory> repository)
        {
            Repository = repository;
        }

        #endregion Constructor

        #region Properties

        public IDocumentDbRepository<OpenSubCategory> Repository { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Kullanıcının açık alt kategorilerinin id listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Sub kategori id listesi</returns>
        public string[] OpenSubCategoryIds(string userId)
        {
            return Repository.Query<string>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE c.subCategoryId FROM c where c.userId = @userId",
                Parameters = new SqlParameterCollection
                 {
                     new SqlParameter("@userId", userId)
                 }
            }).ToArray();
        }

        /// <summary>
        /// Alt kategorinin kilidi açık mı kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <returns>Alt kategori kilidi açık ise true değilse false</returns>
        public bool IsSubCategoryOpen(string userId, string subCategoryId)
        {
            return Repository.Query<bool>(new SqlQuerySpec
            {
                QueryText = "SELECT DISTINCT VALUE NOT(IS_NULL(c.id)) FROM c WHERE c.userId=@userId AND c.subCategoryId=@subCategoryId",
                Parameters = new SqlParameterCollection
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@subCategoryId", subCategoryId)
                }
            }).ToList().FirstOrDefault();
        }

        #endregion Methods
    }
}