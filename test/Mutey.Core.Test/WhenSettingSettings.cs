namespace Mutey.Core.Test
{
    using System.IO.Abstractions.TestingHelpers;
    using Mutey.Core.Settings;
    using Xunit;

    public class WhenSettingSettings
    {
        [ Fact ]
        public void ShouldUpdateRegisteredTargets()
        {
            const string update = "World";
            var flagged = false;

            ISettingsStore store = new SettingsStore( new MockFileSystem() );
            // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
            store.RegisterForNotifications<MockSettings>( args =>
                                                          {
                                                              Assert.Equal( MockSettings.InitialValue, args.OldValue.Value );
                                                              Assert.Equal( update, args.NewValue.Value );
                                                              flagged = true;
                                                          } );

            // ReSharper disable once WithExpressionModifiesAllMembers
            store.Set<MockSettings>( s => s with {Value = update} );

            Assert.True( flagged );
        }

        [ Fact ]
        public void ShouldChangeValue()
        {
            const string newValue = "World";

            ISettingsStore store = new SettingsStore( new MockFileSystem() );

            Assert.NotEqual( newValue, store.Get<MockSettings>().Value );

            // ReSharper disable once WithExpressionModifiesAllMembers
            store.Set<MockSettings>( s => s with {Value = newValue} );
            Assert.Equal( newValue, store.Get<MockSettings>().Value );
        }
    }
}
