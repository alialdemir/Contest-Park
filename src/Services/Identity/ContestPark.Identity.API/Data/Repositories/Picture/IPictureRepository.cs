using ContestPark.Identity.API.Enums;

namespace ContestPark.Identity.API.Data.Repositories.Picture
{
    public interface IPictureRepository
    {
        bool Insert(string userId, string pictureUrl, PictureTypes pictureType);
    }
}
