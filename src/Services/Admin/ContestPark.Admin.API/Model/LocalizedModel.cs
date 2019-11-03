using ContestPark.Core.Enums;

namespace ContestPark.Admin.API.Model
{
    public class LocalizedModel
    {
        public Languages Language { get; set; }

        public string Description { get; set; }

        public string Text { get; set; }
    }
}
