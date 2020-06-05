namespace ContestPark.Identity.API.Enums
{
    //
    // Summary:
    //     Various states of the connection to the internet.
    public enum NetworkAccess
    {
        //
        // Summary:
        //     The state of the connectivity is not known.
        Unknown = 0,

        //
        // Summary:
        //     No connectivity.
        None = 1,

        //
        // Summary:
        //     Local network access only.
        Local = 2,

        //
        // Summary:
        //     Limited internet access.
        ConstrainedInternet = 3,

        //
        // Summary:
        //     Local and Internet access.
        Internet = 4
    }
}
