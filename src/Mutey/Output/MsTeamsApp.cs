using System.Diagnostics;

namespace Mutey.Output
{
    public class MsTeamsApp : ConferencingAppRegistration
    {
        protected override string ProcessName { get; } = "teams.exe";
        protected override string IconName { get; } = "teams.jpg";
        
        public override string Name { get; } = "Microsoft Teams";

        public override IConferenceConnection Connect(Process process)
            => new ConferenceConnection(process,
                new SendKeysCall("Teams", process,
                    0xA4 /*LEFT ALT*/,
                    0xA2 /*LEFT CTRL*/,
                    0x4D /*M*/));
    }
}