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
            process.Exited += OnProcessOnExited;
            process.EnableRaisingEvents = true;
        }

        private void OnProcessOnExited(object? o, EventArgs eventArgs)
            => Closed?.Invoke(this, EventArgs.Empty);

        public ConferenceConnection(Process process, ICall defaultCall) : this(process)
        {
            DefaultCall = defaultCall;
            CanDetectNewCalls = false;
        }

        public bool CanDetectNewCalls { get; }
        public ICall? DefaultCall { get; }

        public event EventHandler<ICall>? CallStarted;
        public event EventHandler? Closed;

        public void Close()
        {
            process.EnableRaisingEvents = false;
        }

        protected void BeginCall(ICall call)
            => CallStarted?.Invoke(this, call);

        public virtual void Dispose() { }
    }
}