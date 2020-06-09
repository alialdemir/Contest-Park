using ContestPark.Mobile.Models.Post;
using Prism.Events;

namespace ContestPark.Mobile.Events
{
    public class PostLikeCountChangeEvent : PubSubEvent<PostModel> { }
}
