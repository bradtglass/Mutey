namespace Mutey.Core.Input
{
    using System;
    using Mutey.Core.Settings;

    public record InputSettings( TransformModes DefaultTransformMode,
                                 TimeSpan InputCooldownDuration,
                                 TimeSpan SmartPttActivationDuration ) : SettingsBase
    {
        public override string Filename => "input";

        public TimeSpan InputCooldownDuration { get; } = InputCooldownDuration;

        public InputSettings() : this( TransformModes.Ptt | TransformModes.Toggle,
                                       TimeSpan.FromMilliseconds( 50 ),
                                       TimeSpan.FromMilliseconds( 500 ) ) { }
    }
}
