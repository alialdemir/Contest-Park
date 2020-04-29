using ContestPark.Core.Models;
using ContestPark.Identity.API.Models;
using Microsoft.EntityFrameworkCore;
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
        /// Kullanıcı bilgilerini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Kullanıcı bilgileri</returns>
        public UserInfoModel UserInfo(string userId)
        {
            return _applicationDbContext
                .Users
                .Where(u => u.Id == userId)
                .Select(u => new UserInfoModel
                {
                    CoverPicturePath = u.CoverPicturePath,
                    ProfilePicturePath = u.ProfilePicturePath,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    IsPrivateProfile = u.IsPrivateProfile,
                    Language = u.Language,
                    UserId = u.Id,
                    Roles = _applicationDbContext.UserRoles.Where(r => r.UserId == userId).Select(x => x.RoleId).ToList()
                })
                .FirstOrDefault();
        }

        /// <summary>
        /// Kullanıcı id'sine ait telefon numarasını verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Kullanıcının telefon numarası</returns>
        public string GetPhoneNumber(string userId)
        {
            return _applicationDbContext
                                    .Users
                                    .Where(u => u.Id == userId)
                                    .Select(u => u.PhoneNumber)
                                    .FirstOrDefault();
        }

        /// <summary>
        /// Telefon numarasına ait kullanıcı adı
        /// </summary>
        /// <param name="phoneNumber">Telefon numarası</param>
        /// <returns>Kullanıcı adı</returns>
        public string GetUserNameByPhoneNumber(string phoneNumber)
        {
            return _applicationDbContext
                .Users
                .FirstOrDefault(x => x.PhoneNumber == phoneNumber)?.UserName;
        }

        /// <summary>
        /// Random bot kullanıcı profil resmi  verir
        /// </summary>
        /// <returns>Kullanıcı profil resimleri</returns>
        public IEnumerable<string> GetRandomProfilePictures()
        {
            string sql = @"SELECT a.ProfilePicturePath FROM AspNetUsers a
                           WHERE a.IsBot = true
                           ORDER BY RAND()
                           LIMIT 30";

            return _applicationDbContext
                .Users
                .FromSqlRaw(sql)
                .Select(u => u.ProfilePicturePath)
                .ToList();
        }

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
        public IEnumerable<UserModel> GetUserInfos(List<string> userInfos, bool includeCoverPicturePath = false)
        {
            return _applicationDbContext
                .Users
                .Where(u => userInfos.Any(s => s == u.Id))
                .Select(u => new UserModel
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    ProfilePicturePath = u.ProfilePicturePath,
                    UserName = u.UserName,
                    CoverPicturePath = includeCoverPicturePath ? u.CoverPicturePath : string.Empty,
                }).ToList();
        }

        /// <summary>
        /// Kullanıcı adına göre profil bilgilerini verir
        /// </summary>
        /// <param name="userName">Kullanıcı adı</param>
        /// <returns>Profil bilgileri</returns>
        public UserProfileModel GetUserInfoByUserName(string userName)
        {
            userName = userName.ToUpper();
            return _applicationDbContext
                .Users
                .Where(u => u.NormalizedUserName.Equals(userName))
                .Select(u => new UserProfileModel
                {
                    UserId = u.Id,
                    FullName = u.FullName,
                    ProfilePicturePath = u.ProfilePicturePath,
                    CoverPicture = u.CoverPicturePath,
                    FollowersCount = u.DisplayFollowersCount,
                    FollowUpCount = u.DisplayFollowingCount,
                    GameCount = u.DisplayGameCount,
                    IsPrivateProfile = u.IsPrivateProfile
                }).FirstOrDefault();
        }

        /// <summary>
        /// Rastgele bot kullanıcı verir
        /// </summary>
        /// <returns>Rastgele bir kullanıcı</returns>
        public RandomUserModel GetRandomBotUser()
        {
            string sql = @"SELECT a.Id, a.ProfilePicturePath, a.FullName FROM AspNetUsers a
                           WHERE a.IsBot = true
                           ORDER BY RAND()
                           LIMIT 1";

            return _applicationDbContext
                                .Users
                                .FromSqlRaw(sql)
                                .Select(u => new RandomUserModel
                                {
                                    UserId = u.Id,
                                    ProfilePicturePath = u.ProfilePicturePath,
                                    FullName = u.FullName,
                                })
                                .FirstOrDefault();
        }

        /// <summary>
        /// Kullanıcı adına ait user id verir
        /// </summary>
        /// <param name="userName">Kullanıcı id</param>
        /// <returns>User id</returns>
        public string GetUserIdByUserName(string userName)
        {
            return _applicationDbContext.Users.FirstOrDefault(x => x.NormalizedUserName == userName)?.Id;
        }

        #endregion Methods
    }
}
