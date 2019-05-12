using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.User;
using JWT;
using JWT.Serializers;

namespace ContestPark.Mobile.Helpers
{
    public static class JwtTokenDecoder
    {
        public static UserInfoModel GetUserInfo(this UserInfoModel userInfoModel, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("fake_token"))// mock data için fake token bilgisine göre user oluşturduk
            {
                return new UserInfoModel()
                {
                    CoverPicturePath = DefaultImages.DefaultCoverPicture,
                    FullName = "Ali Aldemir",
                    Language = Languages.Turkish,
                    ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    UserId = "1111-1111-1111-1111",
                    UserName = "witcherfearless"
                };
            }

            JsonNetSerializer jsonNetSerializer = new JsonNetSerializer();
            JwtBase64UrlEncoder base64UrlEncoder = new JwtBase64UrlEncoder();

            UtcDateTimeProvider utcDateTimeProvider = new UtcDateTimeProvider();
            JwtValidator jwtValidator = new JwtValidator(jsonNetSerializer, utcDateTimeProvider);
            JwtDecoder jwtDecoder = new JwtDecoder(jsonNetSerializer, jwtValidator, base64UrlEncoder);

            userInfoModel = jwtDecoder.DecodeToObject<UserInfoModel>(accessToken);

            return userInfoModel;
        }
    }
}