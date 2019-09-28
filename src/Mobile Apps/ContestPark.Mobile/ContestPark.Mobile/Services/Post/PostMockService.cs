using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.Post.PostLikes;
using ContestPark.Mobile.Models.ServiceModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public class PostMockService : IPostService
    {
        public async Task<bool> DisLikeAsync(int postId)
        {
            await Task.Delay(300);
            return true;
        }

        public Task<PostDetailModel> GetPostByPostIdAsync(int postId, PagingModel pagingModel)
        {
            return Task.FromResult(new PostDetailModel
            {
                Post = new PostModel
                {
                    CommentCount = 13,
                    IsLike = false,
                    Date = DateTime.Now.AddDays(1),
                    LikeCount = 44,
                    PostId = 3,
                    PostType = Enums.PostTypes.Contest,
                    Bet = 123,
                    BalanceType = Enums.BalanceTypes.Gold,

                    SubCategoryId = 1,
                    SubCategoryName = "Football Players",
                    SubCategoryPicturePath = DefaultImages.DefaultLock,

                    CompetitorFullName = "Elif Öz",
                    CompetitorProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    CompetitorUserName = "elfoz",
                    CompetitorTrueAnswerCount = 234,

                    FounderFullName = "Ali Aldemir",
                    FounderProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    FounderTrueAnswerCount = 125,
                    FounderUserName = "witcherfearless",
                },
                Comments = new ServiceModel<PostCommentModel>
                {
                    Count = 5,
                    PageNumber = 1,
                    PageSize = 10,
                    Items = new List<PostCommentModel>
                    {
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                        new PostCommentModel
                        {
                            Comment = "deneme comment",
                            Date= DateTime.Now,
                            FullName="Ali Aldemir",
                            ProfilePicturePath= DefaultImages.DefaultProfilePicture,
                            UserName = "witcherfearless"
                        },
                    }
                },
            });
        }

        public Task<ServiceModel<PostModel>> GetPostsBySubCategoryIdAsync(short subCategoryId, PagingModel pagingModel, bool isForceCache)
        {
            List<PostModel> posts = new List<PostModel>();

            for (int i = pagingModel.PageNumber; i < pagingModel.PageSize + pagingModel.PageNumber; i++)
            {
                posts.Add(new PostModel
                {
                    CommentCount = i,
                    IsLike = false,
                    Date = DateTime.Now.AddDays(-i),
                    LikeCount = i,
                    PostId = 2,
                    PostType = Enums.PostTypes.Contest,
                    Bet = i * 123,
                    BalanceType = i % 2 == 0 ? Enums.BalanceTypes.Gold : Enums.BalanceTypes.Money,

                    SubCategoryId = 1,
                    SubCategoryName = "Football Players",
                    SubCategoryPicturePath = DefaultImages.DefaultLock,

                    CompetitorFullName = "Elif Öz",
                    CompetitorProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    CompetitorUserName = "elfoz",
                    CompetitorTrueAnswerCount = (byte)(i * 20),

                    FounderFullName = "Ali Aldemir",
                    FounderProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    FounderTrueAnswerCount = (byte)(i * 19),
                    FounderUserName = "witcherfearless",
                });
            }

            return Task.FromResult(new ServiceModel<PostModel>
            {
                Count = 2,
                PageNumber = 1,
                PageSize = 1,
                Items = posts
            });
        }

        public Task<ServiceModel<PostModel>> GetPostsByUserIdAsync(string userId, PagingModel pagingModel)
        {
            return Task.FromResult(new ServiceModel<PostModel>
            {
                Count = 2,
                PageNumber = 1,
                PageSize = 1,
                Items = new List<PostModel>
                {
                    new PostModel
                    {
                        CommentCount = 12,
                        IsLike = false,
                        Date = DateTime.Now.AddDays(2),
                        LikeCount = 3,
                        PostId = 1,
                        PostType = Enums.PostTypes.Text,

                        Description = "Merhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünyaMerhaba dünya",

                        OwnerProfilePicturePath = DefaultImages.DefaultProfilePicture,
                        OwnerFullName ="Ali aldemir",
                        OwnerUserName ="Witcherfearless",
                    },
                    new PostModel
                {
                    CommentCount = 12,
                    IsLike = false,
                    Date = DateTime.Now.AddDays(2),
                    LikeCount = 3,
                    PostId = 1,
                    PostType = Enums.PostTypes.Contest,
                    Bet = 2500,

                    SubCategoryId = 1,
                    SubCategoryName = "Football Players",
                    SubCategoryPicturePath = DefaultImages.DefaultLock,

                    CompetitorFullName = "Elif Öz",
                    CompetitorProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    CompetitorUserName = "elfoz",
                    CompetitorTrueAnswerCount =34,

                    FounderFullName = "Ali Aldemir",
                    FounderProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    FounderTrueAnswerCount = 5,
                    FounderUserName = "witcherfearless",
                },

                    new PostModel
                    {
                        CommentCount = 12,
                        IsLike = false,
                        Date = DateTime.Now.AddDays(2),
                        LikeCount = 3,
                        PostId =2,
                        PostType = Enums.PostTypes.Image,

                        Description = "textli yazıtextli yazıtextli yazıtextli yazıtextli yazı textli yazıtextli yazı",

                        OwnerProfilePicturePath = DefaultImages.DefaultProfilePicture,
                        OwnerFullName ="Ali aldemir",
                        OwnerUserName ="Witcherfearless",
                        PicturePath = DefaultImages.DefaultCoverPicture
                    },

                    new PostModel
                    {
                        CommentCount = 12,
                        IsLike = false,
                        Date = DateTime.Now.AddDays(2),
                        LikeCount = 3,
                        PostId =3,
                        PostType = Enums.PostTypes.Image,

                        OwnerProfilePicturePath = DefaultImages.DefaultProfilePicture,
                        OwnerFullName ="Ali aldemir",
                        OwnerUserName ="Witcherfearless",
                        PicturePath = DefaultImages.DefaultCoverPicture
                    },
                }
            });
        }

        public async Task<bool> LikeAsync(int postId)
        {
            await Task.Delay(300);
            return true;
        }

        public Task<ServiceModel<PostLikeModel>> PostLikesAsync(int postId, PagingModel pagingModel)
        {
            List<PostLikeModel> postLikes = new List<PostLikeModel>();

            for (byte i = 0; i < 10; i++)
            {
                postLikes.Add(new PostLikeModel
                {
                    IsFollowing = i % 2 == 0,
                    FullName = "User" + i.ToString(),
                    ProfilePicturePath = DefaultImages.DefaultProfilePicture,
                    UserId = "1111-1111-1111-111" + i.ToString(),
                    UserName = "witcher" + i.ToString(),
                });
            }

            return Task.FromResult(new ServiceModel<PostLikeModel>
            {
                Count = 2,
                PageNumber = 1,
                PageSize = 1,
                Items = postLikes
            });
        }

        public Task<bool> SendCommentAsync(int postId, string comment)
        {
            return Task.FromResult(true);
        }
    }
}
