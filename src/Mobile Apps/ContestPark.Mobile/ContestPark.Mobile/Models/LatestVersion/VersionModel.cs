using System.Collections.Generic;

namespace ContestPark.Mobile.Models.LatestVersion
{
    public class App
    {
        public string Version { get; set; }
    }

    public class VersionIosModel
    {
        public List<VersionResult> Results { get; set; }
    }

    public class VersionResult
    {
        public string Version { get; set; }
        public string TrackViewUrl { get; set; }
    }
}
