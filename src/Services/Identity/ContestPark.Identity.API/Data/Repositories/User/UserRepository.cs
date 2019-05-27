using ContestPark.Identity.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Identity.API.Data.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        #region Private variables

        private readonly ApplicationDbContext _applicationDbContext;

        #endregion Private variables

        #region Constructor

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcı id'ye şifre değiştirme kodu ekler
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Şifre değiştirmek için kod</returns>
        public int InsertCode(string userId)
        {
            int code = new Random().Next(100000, 999999);
            if (IsCodeRegistered(code))// TODO: burada db gitmek yerine her seferinde uniq bir kod oluşturulacak yapı yazılmalı
                return InsertCode(userId);

            _applicationDbContext.ForgetPasswordCodes.Add(new Tables.ForgetPasswordCode
            {
                UserId = userId,
                Code = code
            });

            bool isSuccess = _applicationDbContext.SaveChanges() > 0;

            return isSuccess ? code : 0;
        }

        /// <summary>
        /// Kod daha önce kayıt edilmiş mi kontrol eder
        /// </summary>
        /// <param name="code">Üretilen kode</param>
        /// <returns></returns>
        private bool IsCodeRegistered(int code)
        {
            return _applicationDbContext.ForgetPasswordCodes.Where(p => p.Code == code).Any();
        }

        #endregion Methods
    }
}