using Plugin.Iconize;

namespace ContestPark.Mobile.Helpers
{
    public class ContestParkIconModule : IconModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontAwesomeBrandsModule" /> class.
        /// </summary>
        public ContestParkIconModule()
            : base("ContestPark Icon", "ContestPark Icon", "ContestPark-Icon.ttf", ContestParkIconCollection.SolidIcons)
        {
            // Intentionally left blank
        }
    }
}
