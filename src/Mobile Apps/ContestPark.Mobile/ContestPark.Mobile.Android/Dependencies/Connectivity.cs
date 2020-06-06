using Android.Content;
using Android.Net;
using Android.Telephony;
using ContestPark.Mobile.Dependencies;
using ContestPark.Mobile.Droid.Dependencies;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(Connectivity))]

namespace ContestPark.Mobile.Droid.Dependencies
{
    public class Connectivity : IConnectivity
    {
        public bool IsConnectedFast
        {
            get
            {
                return IsConnectedFast1(CrossCurrentActivity.Current.AppContext);
            }
        }

        /**
	     * Get the network info
	     * @param context
	     * @return
	     */

        private NetworkInfo GetNetworkInfo(Context context)
        {
            ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            return cm.ActiveNetworkInfo;
        }

        /**
	     * Check if there is fast connectivity
	     * @param context
	     * @return
	     */

        private bool IsConnectedFast1(Context context)
        {
            NetworkInfo info = GetNetworkInfo(context);

            TelephonyManager tm = TelephonyManager.FromContext(context);

            return (info != null && info.IsConnected && IsConnectionFast(info.Type, tm.NetworkType));
        }

        /**
	     * Check if the connection is fast
	     * @param type
	     * @param subType
	     * @return
	     */

        private bool IsConnectionFast(ConnectivityType type, NetworkType subType)
        {
            if (type == ConnectivityType.Wifi)
            {
                return true;
            }
            else if (type == ConnectivityType.Mobile)
            {
                switch (subType)
                {
                    //case TelephonyManager.NETWORK_TYPE_1xRTT:
                    case NetworkType.OneXrtt:
                        return false; // ~ 50-100 kbps
                                      //case TelephonyManager.NETWORK_TYPE_CDMA:
                    case NetworkType.Cdma:
                        return false; // ~ 14-64 kbps
                                      //case TelephonyManager.NETWORK_TYPE_EDGE:
                    case NetworkType.Edge:
                        return false; // ~ 50-100 kbps
                                      //case TelephonyManager.NETWORK_TYPE_EVDO_0:
                    case NetworkType.Evdo0:
                        return true; // ~ 400-1000 kbps
                                     //case TelephonyManager.NETWORK_TYPE_EVDO_A:
                    case NetworkType.EvdoA:
                        return true; // ~ 600-1400 kbps
                                     //case TelephonyManager.NETWORK_TYPE_GPRS:
                    case NetworkType.Gprs:
                        return false; // ~ 100 kbps
                                      //case TelephonyManager.NETWORK_TYPE_HSDPA:
                    case NetworkType.Hsdpa:
                        return true; // ~ 2-14 Mbps
                                     //case TelephonyManager.NETWORK_TYPE_HSPA:
                    case NetworkType.Hspa:
                        return true; // ~ 700-1700 kbps
                                     //case TelephonyManager.NETWORK_TYPE_HSUPA:
                    case NetworkType.Hsupa:
                        return true; // ~ 1-23 Mbps
                                     //case TelephonyManager.NETWORK_TYPE_UMTS:
                    case NetworkType.Umts:
                        return true; // ~ 400-7000 kbps
                    /*
	                 * Above API level 7, make sure to set android:targetSdkVersion
	                 * to appropriate level to use these
	                 */
                    //case TelephonyManager.NETWORK_TYPE_EHRPD: // API level 11
                    case NetworkType.Ehrpd:
                        return true; // ~ 1-2 Mbps
                                     //case TelephonyManager.NETWORK_TYPE_EVDO_B: // API level 9
                    case NetworkType.EvdoB:
                        return true; // ~ 5 Mbps
                                     //case TelephonyManager.NETWORK_TYPE_HSPAP: // API level 13
                    case NetworkType.Hspap:
                        return true; // ~ 10-20 Mbps
                                     //case TelephonyManager.NETWORK_TYPE_IDEN: // API level 8
                    case NetworkType.Iden:
                        return false; // ~25 kbps
                                      //case TelephonyManager.NETWORK_TYPE_LTE: // API level 11
                    case NetworkType.Lte:
                        return true; // ~ 10+ Mbps
                                     // Unknown
                                     //case TelephonyManager.NETWORK_TYPE_UNKNOWN:
                    case NetworkType.Unknown:
                        return false;

                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
