using ContestPark.Mobile.Models.User;
using JWT;
using JWT.Serializers;
using System;

namespace ContestPark.Mobile.Helpers
{
    public static class JwtTokenDecoder
    {
        public static UserInfoModel GetUserInfo(string accessToken)
        {
            if (String.IsNullOrEmpty(accessToken))
            {
                return new UserInfoModel();
            }

            JsonNetSerializer jsonNetSerializer = new JsonNetSerializer();
            JwtBase64UrlEncoder base64UrlEncoder = new JwtBase64UrlEncoder();

            UtcDateTimeProvider utcDateTimeProvider = new UtcDateTimeProvider();
            JwtValidator jwtValidator = new JwtValidator(jsonNetSerializer, utcDateTimeProvider);

            var userInfoModel = new JwtDecoder(jsonNetSerializer, jwtValidator, base64UrlEncoder).DecodeToObject<UserInfoModel>(accessToken);

            return userInfoModel;
        }
    }
}