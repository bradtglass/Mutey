using System;
using System.Diagnostics;

namespace Mutey.Output
{
    public class MsTeamsApp : ConferencingAppRegistration
    {
        protected override string ProcessName { get; } = "teams.exe";
        protected override string IconName { get; } = "teams.jpg";
        
        public override string Name { get; } = "Microsoft Teams";

        public override IConferenceConnection Connect(Process process)
            => throw new NotImplementedException();
    }
}