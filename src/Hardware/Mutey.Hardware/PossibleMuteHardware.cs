namespace Mutey.Hardware
{
    using System.Collections.Generic;

    public abstract class PossibleMuteHardware
    {
        public static IEqualityComparer<PossibleMuteHardware> Comparer { get; } = new PossibleMuteHardwareComparer();

        public abstract string FriendlyName { get; }

        public abstract string Type { get; }

        public abstract string LocalIdentifier { get; }

        /// <summary>
        ///     Indicates if the hardware could possibly be another device that is not for use by Mutey.
        /// </summary>
        public abstract bool IsPresumptive { get; }

        public abstract IMuteHardware Connect();

        private sealed class PossibleMuteHardwareComparer : IEqualityComparer<PossibleMuteHardware>
        {
            public bool Equals( PossibleMuteHardware? x, PossibleMuteHardware? y )
            {
                if ( ReferenceEquals( x, y ) ) return true;
                if ( ReferenceEquals( x, null ) ) return false;
                if ( ReferenceEquals( y, null ) ) return false;
                if ( x.GetType() != y.GetType() ) return false;
                return x.FriendlyName == y.FriendlyName && x.Type == y.Type && x.IsPresumptive == y.IsPresumptive;
            }

            public int GetHashCode( PossibleMuteHardware obj )
            {
                unchecked
                {
                    int hashCode = obj.FriendlyName.GetHashCode();
                    hashCode = ( hashCode * 397 ) ^ obj.Type.GetHashCode();
                    hashCode = ( hashCode * 397 ) ^ obj.IsPresumptive.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
