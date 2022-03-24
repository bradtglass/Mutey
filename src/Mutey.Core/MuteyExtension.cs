namespace Mutey.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Windows.Controls;

    public abstract class MuteyExtension 
    {
        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual Version Version => Assembly.GetExecutingAssembly().GetName().Version!;

        protected IMutey Mutey { get; }

        protected MuteyExtension( IMutey mutey )
        {
            Mutey = mutey;
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public virtual TabItem? LoadSettingsPage()
        {
            return null;
        }

        public abstract void Initialise();

        [ SuppressMessage( "ReSharper", "UnusedParameter.Global" ) ]
        protected virtual void Dispose( bool disposing ) { }

        ~MuteyExtension()
        {
            Dispose( false );
        }
    }
}
