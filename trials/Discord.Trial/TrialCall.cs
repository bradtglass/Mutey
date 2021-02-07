using System;
using System.Windows.Automation;
using Mutey.Output;

namespace Discord.Trial
{
    internal class TrialCall : UiAutomationCall
    {
        public TrialCall(string name, IntPtr window) : base(name, window) { }
        protected override void ToggleInternal(AutomationElement muteButton)
        {
            TogglePattern pattern = (TogglePattern) muteButton.GetCachedPattern(TogglePattern.Pattern);
            pattern.Toggle();
        }

        protected override AutomationElement GetMuteButton(AutomationElement element)
            => element.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.NameProperty, "Mute"));
    }
}