using ContestPark.Mobile.Models.Post;
using Prism.Events;

namespace ContestPark.Mobile.Events
{
    public class PostCommentCountChangeEvent : PubSubEvent<PostModel> { }
}
