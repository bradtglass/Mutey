namespace Mutey
{
    using System;
    using Mutey.Core.Settings;
    using Mutey.Popup;

    public record MuteySettings( string? LastDeviceId,
                                 TransformModes DefaultTransformMode,
                                 TimeSpan InputCooldownDuration,
                                 TimeSpan SmartPttActivationDuration,
                                 PopupMode MuteStatePopupMode,
                                 int MuteStatePopupSize ) : SettingsBase
    {
        public override string Filename => "mutey";

        public MuteySettings() : this( null,
                                       TransformModes.Ptt | TransformModes.Toggle,
                                       TimeSpan.FromMilliseconds( 50 ),
                                       TimeSpan.FromMilliseconds( 500 ),
                                       PopupMode.Temporary,
                                       50 ) { }
    }
}
