namespace Mutey.Core
{
    using DryIoc;

    /// <summary>
    ///     Used to register services that are required for a <see cref="MuteyExtension" /> implementation.
    /// </summary>
    public interface IMuteyRegistrator
    {
        /// <summary>
        ///     Registers services.
        /// </summary>
        void Register( IRegistrator registrator );
    }
}
