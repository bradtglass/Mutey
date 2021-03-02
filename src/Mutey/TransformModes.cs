using System;

namespace Mutey
{
    [Flags]
    public enum TransformModes
    {
        Ptt = 1 << 0,
        Toggle = 1 << 1
    }
}
