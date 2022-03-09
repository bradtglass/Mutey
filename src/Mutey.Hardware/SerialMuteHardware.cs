namespace Mutey.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using NLog;

    /// <summary>
    ///     A mute button that communicates changes over serial communication.
    /// </summary>
    public sealed class SerialMuteHardware : IMuteHardware, IDisposable
    {
        private const int hardwareTypeLength = 6;
        private static readonly byte[] toggleButton = {210, 196, 183, 121, 141, 28};
        private static readonly byte[] startType = {57};
        private static readonly byte[] endType = {184};

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly SerialPort port;

        public SerialMuteHardware( PossibleSerialMuteHardware possibleHardware )
        {
            Source = possibleHardware;

            port = new SerialPort( possibleHardware.Port, possibleHardware.BaudRate );

            logger.Info( "Opening connection on serial port {Name}", port.PortName );
            port.Open();

            Thread portWatcherThread = new(WatchPort)
            {
                Priority = ThreadPriority.BelowNormal
            };
            portWatcherThread.Start();
        }


        public void Dispose()
        {
            cancellationTokenSource.Cancel();
            port.Dispose();
        }

        public PossibleMuteHardware Source { get; }

        public event EventHandler<HardwareMessageReceivedEventArgs>? MessageReceived;

        private async void WatchPort()
        {
            try
            {
                await foreach ( byte[] message in ReadMessagesAsync( port.BaseStream, cancellationTokenSource.Token ) )
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if ( MessageReceived == null )
                    {
                        continue;
                    }

                    var (hardwareType, messageType) = ProcessMessage( message );
                    MessageReceived( this, new HardwareMessageReceivedEventArgs( messageType, hardwareType ) );
                }
            }
            catch ( IOException e )
            {
                logger.Warn( e, "Exception captured, this can occur due to disposing of the port whilst awaiting it" );
            }
            catch ( TaskCanceledException ) { }
            catch ( Exception e )
            {
                logger.Error( e, "Unknown exception on port watcher thread" );
            }
        }

        private (HardwareType, HardwareMessageType) ProcessMessage( byte[] message )
        {
            if ( GetHardwareTypeBytes( message ) is not { } hardwareTypeBytes )
            {
                logger.Error( "No hardware type bytes detected" );

                return ( HardwareType.Unknown, HardwareMessageType.Unknown );
            }

            var hardwareType = GetHardwareType( hardwareTypeBytes );

            if ( GetMessageTypeBytes( message ) is not { } messageTypeBytes )
            {
                logger.Error( "No message types bytes detected" );

                return ( hardwareType, HardwareMessageType.Unknown );
            }

            return ( hardwareType, GetMessageType( messageTypeBytes ) );
        }

        private static HardwareType GetHardwareType( byte[] bytes )
        {
            if ( bytes.SequenceEqual( toggleButton ) )
            {
                return HardwareType.Toggle;
            }

            logger.Error( "Unknown hardware byte sequence: {Bytes}", bytes );

            return HardwareType.Unknown;
        }

        private static HardwareMessageType GetMessageType( byte[] bytes )
        {
            if ( bytes.SequenceEqual( startType ) )
            {
                return HardwareMessageType.StartToggle;
            }

            if ( bytes.SequenceEqual( endType ) )
            {
                return HardwareMessageType.EndToggle;
            }

            logger.Error( "Unknown message byte sequence: {Bytes}", bytes );

            return HardwareMessageType.Unknown;
        }

        /// <summary>
        ///     Gets the bytes transmitting the hardware type of <see langword="null" /> is no type was transmitted.
        /// </summary>
        private byte[]? GetHardwareTypeBytes( byte[] message )
        {
            if ( message.Length < hardwareTypeLength )
            {
                return null;
            }

            var result = new byte[ hardwareTypeLength ];
            Array.Copy( message, result, hardwareTypeLength );

            return result;
        }

        /// <summary>
        ///     Gets the bytes transmitting the message type of <see langword="null" /> is no type was transmitted.
        /// </summary>
        private byte[]? GetMessageTypeBytes( byte[] message )
        {
            if ( message.Length <= hardwareTypeLength )
            {
                return null;
            }

            int messageTypeLength = message.Length - hardwareTypeLength;
            var result = new byte[ messageTypeLength ];
            Buffer.BlockCopy( message, 6, result, 0, messageTypeLength );

            return result;
        }

        [ SuppressMessage( "ReSharper", "IteratorNeverReturns" ) ]
        private static async IAsyncEnumerable<byte[]> ReadMessagesAsync( Stream stream,
                                                                         [ EnumeratorCancellation ] CancellationToken cancellationToken )
        {
            List<byte> data = new(7);
            var buffer = new byte[ 8 ];

            while ( true )
            {
                int byteCount = await stream.ReadAsync( buffer, 0, 8, cancellationToken );

                for ( var i = 0; i < byteCount; i++ )
                {
                    byte nextByte = buffer[ i ];

                    if ( nextByte == 0 )
                    {
                        yield return data.ToArray();
                        data = new List<byte>( 7 );
                    }
                    else
                    {
                        data.Add( nextByte );
                    }
                }
            }
        }
    }
}
