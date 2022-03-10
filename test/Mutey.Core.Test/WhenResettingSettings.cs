namespace Mutey.Core.Test
{
    using System.IO.Abstractions.TestingHelpers;
    using Mutey.Core.Settings;
    using Xunit;

    public class WhenResettingSettings
    {
        [ Fact ]
        public void ShouldUpdateRegisteredTargetsIfSettingsAreSet()
        {
            var flagged = false;

            ISettingsStore store = new SettingsStore( new MockFileSystem() );
            store.Set<MockSettings>( s => s ); // Need to create the initial settings

            store.RegisterForNotifications<MockSettings>( _ => flagged = true );

            store.Reset<MockSettings>();

            Assert.True( flagged );
        }
        
        [ Fact ]
        public void ShouldNotUpdateRegisteredTargetsIfSettingsAreNotSet()
        {
            var flagged = false;

            ISettingsStore store = new SettingsStore( new MockFileSystem() );
            store.RegisterForNotifications<MockSettings>( _ => flagged = true );

            store.Reset<MockSettings>();

            Assert.False( flagged );
        }
        
        [ Fact ]
        public void ShouldResetToDefault()
        {
            const string other = "World";

            ISettingsStore store = new SettingsStore( new MockFileSystem() );

            // ReSharper disable once WithExpressionModifiesAllMembers
            store.Set<MockSettings>( s => s with {Value = other} );
            Assert.Equal( other, store.Get<MockSettings>().Value );

            store.Reset<MockSettings>();
            var resetSettings = store.Get<MockSettings>();
            Assert.NotEqual( other, resetSettings.Value );
            Assert.Equal( MockSettings.InitialValue, resetSettings.Value );
        }
    }
}
