namespace Mutey.Hardware
{
    using JetBrains.Annotations;
    using Mutey.Core;

    /// <summary>
    /// An extension providing support for physical hardware interfaces.
    /// </summary>
    [ UsedImplicitly ]
    public class HardwareExtension : MuteyExtension
    {
        public HardwareExtension( IMutey mutey ) : base( mutey ) { }

        public override string Name => "Hardware";

        public override string Description => "Supports physical hardware interfaces for controlling and reporting the mute state";
        
        public override void Initialise()
        {
            
        }
    }
}
