namespace Mutey.Core.Settings
{
    using System.Text.Json.Serialization;

    public abstract record SettingsBase
    {
        /// <summary>
        ///     The name of the file where these sub-settings should be stored.
        /// </summary>
        /// <remarks>This should be a constant value and should not contain reserved characters.</remarks>
        [ JsonIgnore ]
        public abstract string Filename { get; }
    }
}
