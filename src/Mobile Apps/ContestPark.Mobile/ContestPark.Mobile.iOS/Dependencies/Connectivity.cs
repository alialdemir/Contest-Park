using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.iOS.Dependencies;
using Xamarin.Forms;

[assembly: Dependency(typeof(Connectivity))]

namespace ContestPark.Mobile.iOS.Dependencies
{
    public class Connectivity : IConnectivity
    {
        public bool IsConnectedFast
        {
            get { return true; }
        }
    }
}
