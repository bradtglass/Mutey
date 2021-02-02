using System;
using System.Diagnostics;

namespace Mutey.Output
{
    public class ConferenceConnection : IConferenceConnection
    {
        private readonly Process process;

        private ConferenceConnection(Process process)
        {
            this.process = process;
            process.Exited += (_, _) => Closed?.Invoke(this, EventArgs.Empty);
            process.EnableRaisingEvents = true;
        }

        public ConferenceConnection(Process process, ICall defaultCall) : this(process)
        {
            DefaultCall = defaultCall;
            CanDetectNewCalls = false;
        }

        public bool CanDetectNewCalls { get; }
        public ICall? DefaultCall { get; }

        public event EventHandler<ICall>? CallStarted;
        public event EventHandler? Closed;

        protected void BeginCall(ICall call)
            => CallStarted?.Invoke(this, call);
    }
}