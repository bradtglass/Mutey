using System.Collections.Generic;
using NAudio.CoreAudioApi;

namespace Mutey.Hue.Mic;

internal static class Extensions
{
    public static IEnumerable<AudioSessionControl> Enumerate(this SessionCollection collection)
    {
        for (var i = 0; i < collection.Count; i++)
            yield return collection[i];
    }
}