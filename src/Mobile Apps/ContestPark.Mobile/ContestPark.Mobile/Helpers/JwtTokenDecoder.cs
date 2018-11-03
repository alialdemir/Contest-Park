using ContestPark.Mobile.Models.User;
using JWT;
using JWT.Serializers;

namespace ContestPark.Mobile.Helpers
{
    public static class JwtTokenDecoder
    {
        public static UserInfoModel GetUserInfo(this UserInfoModel userInfoModel, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken) || accessToken.Contains("fake_token"))
            {
                return new UserInfoModel();
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