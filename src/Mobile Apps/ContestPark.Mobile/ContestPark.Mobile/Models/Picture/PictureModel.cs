using ContestPark.Mobile.Enums;
using ContestPark.Mobile.Models.Base;

namespace ContestPark.Mobile.Models.Picture
{
    public class PictureModel : IModelBase
    {
        public string PicturePath { get; set; }
        public PictureTypes PictureType { get; set; }
    }
}
