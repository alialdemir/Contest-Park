using ContestPark.Mobile.Models.User;
using Prism.Events;

namespace ContestPark.Mobile.Events
{
    public class ChangeUserInfoEvent : PubSubEvent<UserInfoModel> { }
}
