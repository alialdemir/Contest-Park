using ContestPark.Identity.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Code kayıtlı mı kontrol eder
        /// </summary>
        /// <param name="code">Şifre değiştir kodu</param>
        /// <returns>Kayıtlı ise true değilse false</returns>
        public bool CodeCheck(int code)
        {
            return _applicationDbContext.Users.Any(x => x.ForgetPasswordCode == code);
        }

        /// <summary>
        /// Şifremi unuttum koduna ait  kullanıcıyı getirir
        /// </summary>
        /// <param name="code">Şifremi unuttum kodu</param>
        /// <returns>Kullanıcı</returns>
        public ApplicationUser GetUserByCode(int code)
        {
            return (from u in _applicationDbContext.Users
                    where u.ForgetPasswordCode == code
                    select u).FirstOrDefault();
        }

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

            ApplicationUser user = _applicationDbContext.Users.Where(x => x.Id == userId).First();
            if (user == null)
                return 0;

            user.ForgetPasswordCode = code;

            bool isSuccess = _applicationDbContext.SaveChanges() > 0;

            return isSuccess ? code : 0;
        }

        /// <summary>
        /// Şifremi unuttum kodunu kaldırır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns></returns>
        public void RemoveCode(string userId)
        {
            ApplicationUser user = _applicationDbContext.Users.Where(x => x.Id == userId).First();
            if (user == null)
                return;

            user.ForgetPasswordCode = 0;

            _applicationDbContext.SaveChanges();
        }

        /// <summary>
        /// Kod daha önce kayıt edilmiş mi kontrol eder
        /// </summary>
        /// <param name="code">Üretilen kode</param>
        /// <returns></returns>
        private bool IsCodeRegistered(int code)
        {
            return _applicationDbContext.Users.Where(p => p.ForgetPasswordCode == code).Any();
        }

        /// <summary>
        /// Kullanıcı idlerine ait ad soyad, kullanıcı adı, profil resmi, user id gibi bilgileri döner
        /// </summary>
        /// <param name="userInfos">Kullanıcı idleri</param>
        /// <returns>Kullanıcı bilgileri</returns>
        public IEnumerable<UserNotFoundModel> GetUserInfos(List<string> userInfos)
        {
            return _applicationDbContext
                .Users
                .Where(u => userInfos.Any(s => s == u.Id))
                .Select(u => new UserNotFoundModel
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    ProfilePicturePath = u.ProfilePicturePath,
                    UserName = u.UserName
                }).ToList();
        }

        #endregion Methods
    }
}