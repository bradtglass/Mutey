using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Automation;
using NLog;

namespace Mutey.Output
{
    public class DiscordAppWideCall : UiAutomationCall
    {
        private const ToggleState muted = ToggleState.On;
        private const ToggleState unmuted = ToggleState.Off;

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly Lazy<AutomationElement> cachedMuteButtonElement;

        public DiscordAppWideCall(Process process) : base(process.MainWindowHandle)
        {
            process.EnableRaisingEvents = true;
            process.Exited += (_, _) => End();

            cachedMuteButtonElement = new Lazy<AutomationElement>(GetMuteButton);

            // Start initialization of the element in a background thread
            Task.Run(() => cachedMuteButtonElement.Value);
        }

        public override string Name { get; } = "Discord";
        public override bool CanToggle { get; } = true;
        public override bool CanMuteUnmute { get; } = true;
        public override bool CanRaiseMuteStateChanged { get; } = false;

        private static AutomationElement GetMuteButton(AutomationElement element)
            => element.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Mute"));

        private AutomationElement GetMuteButton()
        {
            AutomationElement windowElement = GetWindowElement();

            return GetMuteButton(windowElement);
        }

        public override void Toggle()
        {
            TogglePattern pattern = GetCurrentTogglePattern();

            CommitAutomationInput(() => pattern.Toggle());
        }

        public override void Mute()
            => SetToggleState(muted);

        private void SetToggleState(ToggleState state)
        {
            if (GetToggleState() == state)
                return;

            Toggle();
        }

        private AutomationElement GetCachedMuteButton()
            => cachedMuteButtonElement.Value;

        private TogglePattern GetCurrentTogglePattern()
            => (TogglePattern) GetCachedMuteButton().GetCurrentPattern(TogglePattern.Pattern);

        private ToggleState GetToggleState()
            => GetCurrentTogglePattern().Current.ToggleState;

        public override void Unmute()
            => SetToggleState(unmuted);

        public override MuteState GetState()
        {
            ToggleState state = GetToggleState();
            switch (state)
            {
                case muted:
                    return MuteState.Muted;
                case unmuted:
                    return MuteState.Unmuted;
                default:
                    logger.Warn("Unexpected toggle state: {State}", state);
                    return MuteState.Unknown;
            }
        }

        public override event EventHandler? MuteStateChanged;
    }
}