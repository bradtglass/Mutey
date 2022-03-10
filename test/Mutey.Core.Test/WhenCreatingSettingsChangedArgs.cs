namespace Mutey.Core.Test
{
    using Mutey.Core.Settings;
    using Xunit;

    public class WhenCreatingSettingsChangedArgs
    {
        [ Fact ]
        public void ShouldCreateFromTypeAndObjects()
        {
            const int oldValue = 78;
            const int newValue = 687923;

            var args = SettingsChangedEventArgs.Create( oldValue.GetType(), oldValue, newValue );

            var intArgs = Assert.IsType<SettingsChangedEventArgs<int>>( args );
            Assert.Equal( oldValue, intArgs.OldValue );
            Assert.Equal( newValue, intArgs.NewValue );
        }
    }
}
