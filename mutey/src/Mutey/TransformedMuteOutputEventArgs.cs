using System;

namespace Mutey
{
    public class TransformedMuteOutputEventArgs : EventArgs
    {
        public TransformedMuteOutputEventArgs(MuteAction action, bool isInPtt)
        {
            Action = action;
            IsInPtt = isInPtt;
        }

        public MuteAction Action { get; }
        public bool IsInPtt { get; }
    }
}