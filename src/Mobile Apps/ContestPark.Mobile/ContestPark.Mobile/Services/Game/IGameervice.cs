using ContestPark.Mobile.Models.Categories;
using Prism.Navigation;
using System.Threading.Tasks;

namespace ContestPark.Mobile.Services.Game
{
    public interface IGameervice
    {
        Task<bool> PushCategoryDetailViewAsync(SubCategoryModel subCategoryModel);

        INavigationService NavigationService { get; set; }
    }
}