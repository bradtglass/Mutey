namespace Mutey
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using DryIoc;
    using Mutey.Core;
    using NLog;
    using Prism.DryIoc;
    using Prism.Ioc;
    using Prism.Modularity;

    /// <summary>
    ///     Delegates registration and initialisation to extensions.
    /// </summary>
    public class ExtensionLoaderModule : IModule
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly Lazy<IReadOnlyCollection<Assembly>> lazyAssemblies = new(SearchForAssemblies);

        public void RegisterTypes( IContainerRegistry containerRegistry )
        {
            logger.Info( "Registering extension types" );

            var container = containerRegistry.GetContainer();
            foreach ( var type in SearchForTypes<IMuteyRegistrator>() )
            {
                try
                {
                    logger.Trace( "Registering types for from {Type}", type );
                    var registrator = (IMuteyRegistrator) Activator.CreateInstance( type )!;

                    registrator.Register( container );
                }
                catch ( Exception e )
                {
                    logger.Error( e, "Failed to activate registrator {Type}", type );

                    throw;
                }
            }

            foreach ( var type in SearchForTypes<MuteyExtension>() )
            {
                container.Register( typeof( MuteyExtension ), type, Reuse.Singleton );
            }
        }

        public void OnInitialized( IContainerProvider containerProvider )
        {
            logger.Info( "Initialising extensions" );
            
            
            foreach ( var type in SearchForTypes<MuteyExtension>() )
            {
                try
                {
                    logger.Trace( "Resolving extension {Type}", type );
                    var extension = (MuteyExtension)containerProvider.Resolve(type);

                    logger.Trace( "Initialising extension {Type}", type );
                    extension.Initialise();
                }
                catch ( Exception e )
                {
                    logger.Error( e, "Failed to activate registrator {Type}", type );

                    throw;
                }
            }
        }

        private static IReadOnlyCollection<Assembly> SearchForAssemblies()
        {
            logger.Trace( "Beginning assembly search" );
            string assemblyFile = Assembly.GetExecutingAssembly().Location;
            string? directory = Path.GetDirectoryName( assemblyFile );

            List<Assembly> assemblies = new();

            logger.Debug( "Searching {Directory} for extension assemblies", directory );
            if ( directory is { } && Directory.Exists( directory ) )
            {
                foreach ( string file in Directory.EnumerateFiles( directory )
                                                  .Where( f => f.EndsWith( ".exe", StringComparison.OrdinalIgnoreCase ) ||
                                                               f.EndsWith( ".dll", StringComparison.OrdinalIgnoreCase ) ) )
                {
                    try
                    {
                        logger.Trace( "Loading {File}" );
                        var assembly = Assembly.LoadFrom( file );

                        assemblies.Add( assembly );
                    }
                    catch ( Exception e )
                    {
                        logger.Error( e, "Failed to load {File}", file );
                    }
                }
            }

            return assemblies;
        }

        private IEnumerable<Type> SearchForTypes<T>()
        {
            return lazyAssemblies.Value.SelectMany( a => a.GetExportedTypes() )
                                 .Where( t => !t.IsAbstract )
                                 .Where( t => t.IsAssignableTo( typeof( T ) ) );
        }
    }
}
