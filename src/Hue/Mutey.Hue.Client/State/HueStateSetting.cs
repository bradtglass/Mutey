namespace Mutey.Hue.Client.State
{
    using System.Collections.Generic;

    public record HueStateSetting( IReadOnlyDictionary<string, HueLightSetting> Lights )
    {
        public static readonly HueStateSetting Empty = new(new Dictionary<string, HueLightSetting>());
    }
}
