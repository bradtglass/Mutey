namespace Mutey
{
    using System;

    public class TransformedMuteOutputEventArgs : EventArgs
    {
        public MuteAction Action { get; }
        public bool IsInPtt { get; }

        public TransformedMuteOutputEventArgs( MuteAction action, bool isInPtt )
        {
            Action = action;
            IsInPtt = isInPtt;
        }
    }
}
