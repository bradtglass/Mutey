namespace Mutey.Hardware.Serial
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
    using Mutey.Core.Input;
    using NLog;

    /// <summary>
    ///     A mute button that communicates changes over serial communication.
    /// </summary>
    public sealed class SerialMuteDevice : IMuteDevice, IDisposable
    {
        private const int deviceTypeLength = 6;
        private static readonly byte[] toggleButton = {210, 196, 183, 121, 141, 28};
        private static readonly byte[] startType = {57};
        private static readonly byte[] endType = {184};

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly CancellationTokenSource cancellationTokenSource = new();

        private readonly SerialPort port;

        public SerialMuteDevice( PossibleSerialMuteDevice possibleDevice )
        {
            Source = possibleDevice;

            port = new SerialPort( possibleDevice.Port, possibleDevice.BaudRate );

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

        public PossibleMuteDevice Source { get; }

        public event EventHandler<InputMessageEventArgs>? MessageReceived;

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

                    var messageType = ProcessMessage( message );
                    MessageReceived( this, new InputMessageEventArgs( messageType, Source.LocalIdentifier ) );
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

        private InputMessageKind ProcessMessage( byte[] message )
        {
            if ( GetDeviceKindBytes( message ) is not { } deviceKindBytes )
            {
                logger.Error( "No device kind bytes detected" );

                return InputMessageKind.Unknown;
            }

            var deviceKind = GetDeviceKind( deviceKindBytes );

            if ( deviceKind == DeviceKind.Unknown )
            {
                logger.Error( "Cannot process a mesaage for an unknown device kind" );

                return InputMessageKind.Unknown;
            }

            if ( GetMessageTypeBytes( message ) is not { } messageTypeBytes )
            {
                logger.Error( "No message types bytes detected" );

                return InputMessageKind.Unknown;
            }

            return GetMessageKind( messageTypeBytes );
        }

        private static DeviceKind GetDeviceKind( byte[] bytes )
        {
            if ( bytes.SequenceEqual( toggleButton ) )
            {
                return DeviceKind.Toggle;
            }

            logger.Error( "Unknown hardware byte sequence: {Bytes}", bytes );

            return DeviceKind.Unknown;
        }

        private static InputMessageKind GetMessageKind( byte[] bytes )
        {
            if ( bytes.SequenceEqual( startType ) )
            {
                return InputMessageKind.StartToggle;
            }

            if ( bytes.SequenceEqual( endType ) )
            {
                return InputMessageKind.EndToggle;
            }

            logger.Error( "Unknown message byte sequence: {Bytes}", bytes );

            return InputMessageKind.Unknown;
        }

        /// <summary>
        ///     Gets the bytes transmitting the hardware type of <see langword="null" /> is no type was transmitted.
        /// </summary>
        private byte[]? GetDeviceKindBytes( byte[] message )
        {
            if ( message.Length < deviceTypeLength )
            {
                return null;
            }

            var result = new byte[ deviceTypeLength ];
            Array.Copy( message, result, deviceTypeLength );

            return result;
        }

        /// <summary>
        ///     Gets the bytes transmitting the message type of <see langword="null" /> is no type was transmitted.
        /// </summary>
        private byte[]? GetMessageTypeBytes( byte[] message )
        {
            if ( message.Length <= deviceTypeLength )
            {
                return null;
            }

            int messageTypeLength = message.Length - deviceTypeLength;
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
