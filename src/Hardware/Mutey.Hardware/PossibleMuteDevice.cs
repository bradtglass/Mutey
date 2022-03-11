namespace Mutey.Hardware
{
    using System.Collections.Generic;

    public abstract class PossibleMuteDevice
    {
        public static IEqualityComparer<PossibleMuteDevice> Comparer { get; } = new PossibleMuteDeviceComparer();

        public abstract string FriendlyName { get; }

        public abstract string Type { get; }

        public abstract string LocalIdentifier { get; }

        /// <summary>
        ///     Indicates if the hardware could possibly be another device that is not for use by Mutey.
        /// </summary>
        public abstract bool IsPresumptive { get; }

        public abstract IMuteDevice Connect();

        private sealed class PossibleMuteDeviceComparer : IEqualityComparer<PossibleMuteDevice>
        {
            public bool Equals( PossibleMuteDevice? x, PossibleMuteDevice? y )
            {
                if ( ReferenceEquals( x, y ) ) return true;
                if ( ReferenceEquals( x, null ) ) return false;
                if ( ReferenceEquals( y, null ) ) return false;
                if ( x.GetType() != y.GetType() ) return false;
                return x.FriendlyName == y.FriendlyName && x.Type == y.Type && x.IsPresumptive == y.IsPresumptive;
            }

            public int GetHashCode( PossibleMuteDevice obj )
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
