using System.Diagnostics;

namespace Mutey.Output
{
    public class DiscordApp : ConferencingAppRegistration
    {
        protected override string ModuleName { get; } = "Discord.exe";
        protected override string IconName { get; } = "discord.jpg";
        
        public override string Name { get; } = "Discord";

        public override IConferenceConnection Connect(Process process)
            => new ConferenceConnection(process, new DiscordAppWideCall(process));
    }
}