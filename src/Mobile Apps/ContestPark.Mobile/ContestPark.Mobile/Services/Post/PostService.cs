﻿using ContestPark.Mobile.Configs;
using ContestPark.Mobile.Helpers;
using ContestPark.Mobile.Models.PagingModel;
using ContestPark.Mobile.Models.Post;
using ContestPark.Mobile.Models.ServiceModel;
using ContestPark.Mobile.Services.Cache;
using ContestPark.Mobile.Services.RequestProvider;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Post
{
    public class PostService : IPostService
    {
        #region Private variables

        private const string ApiUrlBase = "api/v1/post";
        private readonly ICacheService _cacheService;
        private readonly IRequestProvider _requestProvider;

        #endregion Private variables

        #region Constructor

        public PostService(IRequestProvider requestProvider,
                           ICacheService cacheService)
        {
            _requestProvider = requestProvider;
            _cacheService = cacheService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Post beğenmekten vazgeç
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> DisLikeAsync(string postId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}");

            try
            {
                await _requestProvider.PostAsync<string>(uri);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }

        /// <summary>
        /// Post beğe
        /// </summary>
        /// <param name="postId">Post id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public async Task<bool> LikeAsync(string postId)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}");

            try
            {
                await _requestProvider.DeleteAsync<string>(uri);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }

        /// <summary>
        /// Alt kategoriye ait postlar
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori postları</returns>
        public async Task<ServiceModel<PostModel>> SubCategoryPostsAsync(short subCategoryId, PagingModel pagingModel)
        {
            string uri = UriHelper.CombineUri(GlobalSetting.Instance.GatewaEndpoint, $"{ApiUrlBase}/subcategory/{subCategoryId}{pagingModel.ToString()}");

            if (!_cacheService.IsExpired(key: uri))
            {
                return await _cacheService.Get<ServiceModel<PostModel>>(uri);
            }

            var posts = await _requestProvider.GetAsync<ServiceModel<PostModel>>(uri);

            _cacheService.Add(uri, posts);

            return posts;
        }

        #endregion Methods
    }
}