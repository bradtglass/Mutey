namespace Mutey
{
    public enum UpdateState
    {
        /// <summary>
        ///     The installed version is the latest.
        /// </summary>
        Latest,

        /// <summary>
        ///     The new version is available.
        /// </summary>
        Available,

        /// <summary>
        ///     The update service is currently checking for an update.
        /// </summary>
        Checking,

        /// <summary>
        ///     The version has not been checked.
        /// </summary>
        NotChecked
    }
}