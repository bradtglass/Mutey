namespace Mutey
{
    using Mutey.Core.Settings;
    using Mutey.Popup;

    public record ViewSettings( PopupMode MuteStatePopupMode,
                                int MuteStatePopupSize ) : SettingsBase
    {
        public override string Filename => "view";

        public ViewSettings() : this( PopupMode.Temporary,
                                      50 ) { }
    }
}
