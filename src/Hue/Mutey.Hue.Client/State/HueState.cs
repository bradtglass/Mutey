namespace Mutey.Hue.Client.State
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class HueState
    {
        public static IHueState Get( IReadOnlyCollection<IHueState> states )
        {
            return states.Count switch
            {
                0 => NoOpHueState.Instance,
                1 => states.First(),
                _ => new HueStateWrapper( states )
            };
        }

        public static async ValueTask<IHueState> CreateAsync( HueStateSetting setting, HueContext context )
        {
            List<IHueState> states = new();
            foreach ( var lightGroup in setting.Lights
                                               .GroupBy( l => l.Value ) )
            {
                var state = await BuildStateForLightGroupAsync( context, lightGroup.Key, lightGroup.Select( kvp => kvp.Key ) );

                if ( state != null )
                {
                    states.Add( state );
                }
            }

            return Get( states );
        }


        private static async ValueTask<IHueState?> BuildStateForLightGroupAsync( HueContext context, HueLightSetting setting,
                                                                                 IEnumerable<string> lights )
        {
            List<string> ids = new();

            foreach ( string light in lights )
            {
                if ( await context.FindLightIdAsync( light ) is { } id )
                {
                    ids.Add( id );
                }
            }

            if ( ids.Count == 0 )
            {
                return null;
            }

            return new HueLightState( setting, ids );
        }
    }
}
