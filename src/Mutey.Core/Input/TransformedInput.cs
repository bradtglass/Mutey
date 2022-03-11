namespace Mutey.Core.Input
{
    using Mutey.Core.Audio;

    public readonly struct TransformedInput
    {
        public static TransformedInput None => new(MuteAction.None, false);

        public MuteAction Action { get; }

        public bool IsInPtt { get; }

        public TransformedInput( MuteAction action, bool isInPtt )
        {
            Action = action;
            IsInPtt = isInPtt;
        }
    }
}
