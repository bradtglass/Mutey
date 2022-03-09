namespace Mutey.Hue.Client.State
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class NoOpHueState : IHueState
    {
        public static NoOpHueState Instance { get; } = new();
        private NoOpHueState() { }

        public Task ApplyAsync( HueContext _ )
        {
            return Task.CompletedTask;
        }

        public IEnumerable<string> GetAffectedLights()
        {
            return Array.Empty<string>();
        }
    }
}
