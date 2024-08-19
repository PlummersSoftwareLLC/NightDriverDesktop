//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2019 Dave Plummer.  All Rights Reserved.
//
// File:        LEDControllerChannel.cs
//
// Description:
//
//   Represents a specific channel on a particular strip and exposes the
//   GraphicsBase class for drawing directly on it.  
//
//   Each instance has a worker thread that manages keeping the socket 
//   connected and sending it the data that has been queued up for it.
//
// History:     Jun-15-2019        Davepl      Created
//
//---------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


// LEDControllerChannel
//
// Exposes ILEDGraphics via the GraphicsBase baseclass
// Abstract until deriving class implements GetDataFrame()

namespace NightDriver
{

    [Serializable]
    public struct SocketResponse
    {
        public uint size;
        public uint flashVersion;
        public double currentClock;
        public double oldestPacket;
        public double newestPacket;
        public double brightness;
        public double wifiSignal;
        public uint bufferSize;
        public uint bufferPos;
        public uint fpsDrawing;
        public uint watts;


        public void Reset()
        {
            size = 0;
            flashVersion = 0;
            currentClock = 0;
            oldestPacket = 0;
            newestPacket = 0;
            brightness = 0;
            wifiSignal = 0;
            bufferSize = 0;
            bufferPos = 0;
            fpsDrawing = 0;
            watts = 0;
        }
    };

    [Serializable]
    public class LEDControllerChannel
    {
        private CancellationTokenSource _cancellationTokenSource;

        public string HostName { get; set; }
        public string FriendlyName { get; set; }
        public bool CompressData { get; set; } = true;
        public byte Channel { get; set; } = 0;
        public double Brightness { get; set; } = 1.0f;
        public uint Connects { get; set; } = 0;
        public uint Watts { get; set; } = 0;
        public bool RedGreenSwap { get; set; } = false;
        public uint BatchSize { get; set; } = 1;
        public double BatchTimeout { get; set; } = 1.00;


        [JsonIgnore]
        public Site StripSite { get; set; }

        [JsonIgnore]
        public SocketResponse Response;

        private ConcurrentQueue<byte[]> DataQueue = new ConcurrentQueue<byte[]>();

        private Thread _Worker;

        [JsonIgnore]
        public int QueueDepth
        {
            get
            {
                return DataQueue.Count;
            }
        }
        public const int MaxQueueDepth = 99;

        public uint Offset
        {
            get;
            set;
        }

        public uint Width
        {
            get;
            set;
        }

        public uint Height
        {
            get;
            set;
        }

        protected LEDControllerChannel(string hostName,
                                       string friendlyName,
                                       uint width,
                                       uint height = 1,
                                       uint offset = 0,
                                       bool compressData = true,
                                       byte channel = 0,
                                       byte watts = 0,
                                       bool swapRedGreen = false,
                                       uint batchSize = 1)
        {
            HostName = hostName;
            FriendlyName = friendlyName;
            Width = width;
            Height = height;
            Channel = channel;
            Offset = offset;
            Watts = watts;
            CompressData = compressData;
            RedGreenSwap = swapRedGreen;
            BatchSize = batchSize;
        }

        ~LEDControllerChannel()
        {
            Stop();
        }

        public bool Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _Worker = new Thread(WorkerConnectAndSendLoop);
            _Worker.Name = HostName + " Connect and Send Loop";
            _Worker.IsBackground = true;
            _Worker.Priority = ThreadPriority.BelowNormal;
            _Worker.Start();
            return true;
        }

        // Stop
        //
        // Signals the worker thread to exit and closes the socket (if open)

        public bool Stop()
        {
            // If we don't have a cancellation source, we've been told to stop before startingl likely

            if (null == _cancellationTokenSource)
                return false;

            // Shut down the controller socket for this strip

            ControllerSocket controllerSocket = ControllerSocketForHost(HostName);
            if (controllerSocket != null && controllerSocket._socket != null)
                controllerSocket.Close();

            // Wrap up the worker thread

            _cancellationTokenSource.Cancel();
            return true;
        }

        [JsonIgnore]
        public bool HasSocket       // Is there a socket at all yet?
        {
            get
            {
                ControllerSocket controllerSocket = ControllerSocketForHost(HostName);
                if (null == controllerSocket || controllerSocket._socket == null)
                    return false;
                return controllerSocket._socket.Connected;
            }
        }

        [JsonIgnore]
        public bool ReadyForData    // Is there a socket and is it connected to the chip?
        {
            get
            {
                ControllerSocket controllerSocket = ControllerSocketForHost(HostName);
                if (null == controllerSocket ||
                    controllerSocket._socket == null ||
                    !controllerSocket._socket.Connected ||
                    QueueDepth > MaxQueueDepth)
                    return false;
                return true;
            }
        }

        [JsonIgnore]
        public uint MinimumSpareTime => _HostControllerSockets.Count > 0 ? _HostControllerSockets.Min(controller => controller.Value.BytesPerSecond) : 0;

        [JsonIgnore]
        public uint BytesPerSecond
        {
            get
            {
                ControllerSocket controllerSocket = ControllerSocketForHost(HostName);
                if (null == controllerSocket || controllerSocket._socket == null)
                    return 0;
                return controllerSocket.BytesPerSecond;
            }
        }
        [JsonIgnore]
        public uint TotalBytesPerSecond => _HostControllerSockets.Count > 0 ? (uint)_HostControllerSockets.Sum(controller => controller.Value.BytesPerSecond) : 0;

        [JsonIgnore]
        public ControllerSocket ControllerSocket
        {
            get
            {
                return ControllerSocketForHost(HostName);
            }
        }

        public static ControllerSocket ControllerSocketForHost(string host)
        {
            if (_HostControllerSockets.ContainsKey(host))
            {
                _HostControllerSockets.TryGetValue(host, out ControllerSocket controller);
                return controller;
            }
            return null;
        }

        /* BUGBUG Could be abstract except for serialization */

        protected virtual byte[] GetDataFrame(CRGB[] MainLEDs, DateTime timeStart)
        {
            throw new ApplicationException("Should never hit base class GetDataFrame");
        }

        byte[] CompressFrame(byte[] data)
        {
            const int COMPRESSED_HEADER_TAG = 0x44415645;       // Magic "DAVE" tag for compressed data - replaces size field
            byte[] compressedData = LEDInterop.Compress(data);
            byte[] compressedFrame = LEDInterop.CombineByteArrays(LEDInterop.DWORDToBytes(COMPRESSED_HEADER_TAG),
                                                                  LEDInterop.DWORDToBytes((uint)compressedData.Length),
                                                                  LEDInterop.DWORDToBytes((uint)data.Length),
                                                                  LEDInterop.DWORDToBytes(0x12345678),
                                                                  compressedData);
            return compressedFrame;
        }

        DateTime _timeLastSend = DateTime.UtcNow;
        DateTime _lastBatchTime = DateTime.UtcNow;

        // _HostSockets
        //
        // We can only have one socket open per ESP32 chip per channel, so this concurrent dictionary keeps track of which sockets are
        // open to what chips so that the socket can be reused.  

        static ConcurrentDictionary<string, ControllerSocket> _HostControllerSockets = new ConcurrentDictionary<string, ControllerSocket>();

        private int _iPacketCount = 0;

        public bool CompressAndEnqueueData(CRGB[] MainLEDs, DateTime timeStart)
        {
            // If there is already a socket open, we will use that; otherwise, a new connection will be opened and if successful
            // it will be placed into the _HostSockets concurrent dictionary

            if (_HostControllerSockets.ContainsKey(HostName) == false && (DateTime.UtcNow - _timeLastSend).TotalSeconds < 2)
            {
                ConsoleApp.Stats.WriteLine("Too early to retry for " + HostName);
                return false;
            }
            _timeLastSend = DateTime.UtcNow;

            ControllerSocket controllerSocket = ControllerSocketForHost(HostName);
            if (null == controllerSocket)
                _HostControllerSockets[HostName] = controllerSocket;

            if (QueueDepth > MaxQueueDepth)
            {
                ConsoleApp.Stats.WriteLine("Queue full so dicarding frame for " + HostName);
                return false;
            }

            // Swap color bytes for strips that need it 

            if (RedGreenSwap)
            {
                foreach (var led in MainLEDs)
                {
                    var temp = led.r;
                    led.r = led.g;
                    led.g = temp;
                }
            }

            // Optionally compress the data, but when we do, if the compressed is larger, we send the original

            byte[] msgraw = GetDataFrame(MainLEDs, timeStart);
            byte[] msg = CompressData ? CompressFrame(msgraw) : msgraw;
            if (msg.Length >= msgraw.Length)
            {
                msg = msgraw;
            }

            DataQueue.Enqueue(msg);
            _iPacketCount++;
            return true;
        }

        bool ShouldSendBatch
        {
            get
            {
                if (StripSite is null)
                    return false;

                if (DataQueue.Count() > StripSite.FramesPerSecond)                   // If a full second has accumulated
                    return true;

                if (DataQueue.Any())
                    if ((DateTime.UtcNow - _lastBatchTime).TotalSeconds > BatchTimeout)
                        return true;

                if (DataQueue.Count() >= BatchSize)
                    return true;

                return false;
            }
        }
        // WorkerConnectAndSendLoop
        //
        // Every controller has a worker thread that sits and spins in a thread loop doing the work of connecting to
        // the chips, pulling data from the queues, and sending it off

        void WorkerConnectAndSendLoop()
        {
            while (_cancellationTokenSource.IsCancellationRequested == false)
            {
                ControllerSocket controllerSocket
                    = _HostControllerSockets.GetOrAdd(HostName, (hostname) =>
                      {
                          Connects++;
                          //ConsoleApp.Stats.WriteLine("Connecting to " + HostName);
                          return new ControllerSocket(hostname);
                      });


                if (QueueDepth >= MaxQueueDepth)
                {
                    DataQueue.Clear();
                    ConsoleApp.Stats.WriteLine("Discarding data for jammed socket: " + HostName);
                    ControllerSocket oldSocket;
                    _HostControllerSockets.TryRemove(HostName, out oldSocket);
                    continue;
                }


                if (false == controllerSocket.EnsureConnected())
                {
                    //ConsoleApp.Stats.WriteLine("Closing disconnected socket: " + HostName);
                    ControllerSocket oldSocket;
                    _HostControllerSockets.TryRemove(HostName, out oldSocket);
                    continue;
                }

                // Compose a message which is a binary block of N (where N is up to Count) dequeue packets all
                // in a row, which is how the chips can actually process them

                if (ShouldSendBatch)
                {
                    _lastBatchTime = DateTime.UtcNow;

                    byte[] msg = LEDInterop.CombineByteArrays(DataQueue.DequeueChunk(DataQueue.Count()).ToArray());
                    if (msg.Length > 0)
                    {
                        try
                        {
                            uint bytesSent = 0;
                            if (!controllerSocket.IsDead)
                                bytesSent = controllerSocket.SendData(msg, ref Response);

                            if (bytesSent != msg.Length)
                            {
                                ConsoleApp.Stats.WriteLine("Could not write all bytes so closing socket for " + HostName);
                                ControllerSocket oldSocket;
                                _HostControllerSockets.TryRemove(HostName, out oldSocket);
                            }
                            else
                            {
                                // Console.WriteLine("Sent " + bytesSent + " to " + HostName);
                                double framesPerSecond = (double)(DateTime.UtcNow - _timeLastSend).TotalSeconds;
                            }
                        }
                        catch (SocketException ex)
                        {
                            ConsoleApp.Stats.WriteLine("Exception writing to socket for " + HostName + ": " + ex.Message);
                            ControllerSocket oldSocket;
                            _HostControllerSockets.TryRemove(HostName, out oldSocket);
                        }
                    }
                }
                Thread.Sleep(10);
            }
            Debug.WriteLine("Leaving WorkerConnectAndSendLoop");

        }
    }

    // ControllerSocket
    //
    // Wrapper for .Net Socket so that we can track the number of bytes sent and so on

    [Serializable]
    public class ControllerSocket
    {
        public Socket _socket;
        private IPAddress _ipAddress;
        private IPEndPoint _remoteEP;

        private DateTime LastDataFrameTime;

        private uint BytesSentSinceFrame = 0;
        public string HostName { get; set; }

        public bool IsDead { get; protected set; } = false;
        public string FirmwareVersion { get; set; }

        public uint BytesPerSecond
        {
            get
            {
                double d = (DateTime.UtcNow - LastDataFrameTime).TotalSeconds;
                if (d < 0.001)
                    return 0;

                return (uint)(BytesSentSinceFrame / d);
            }
        }

        public ControllerSocket(string hostname)
        {
            var entry = Dns.GetHostByName(hostname);
            _ipAddress = entry.AddressList[0];
            _remoteEP = new IPEndPoint(_ipAddress, 49152);
            return;

            // I also wrote the async version, but am staying sync above for simplicity right now
            //
            // HostName = hostname;
            // ConsoleApp.Stats.WriteLine("Fetching hostnamae for " + hostname);
            // _remoteEP = null;
            // Dns.BeginGetHostAddresses(HostName, OnDnsGetHostAddressesComplete, this);
        }

        private void OnDnsGetHostAddressesComplete(IAsyncResult result)
        {
            try
            {
                if (result.IsCompleted)
                {
                    var This = (ControllerSocket)result.AsyncState;
                    This._ipAddress = Dns.EndGetHostAddresses(result)[0];
                    This._remoteEP = new IPEndPoint(_ipAddress, 49152);
                    ConsoleApp.Stats.WriteLine("Got IP of " + _remoteEP.Address.ToString() + " for  " + This.HostName);
                }
                else
                    IsDead = true;
            }
            catch (Exception)
            {
                ConsoleApp.Stats.WriteLine("DNS Exception: " + HostName);
                IsDead = true;
            }
        }

        // EnsureConnected
        //
        // If not already connected, initiates the connection so that perhaps next time we will ideally be connected

        public bool EnsureConnected()
        {
            if (IsDead == true)
                return false;

            if (_socket != null && _socket.Connected)
                return true;

            if (_remoteEP == null)
                return false;

            try
            {
                if (DateTime.UtcNow - LastDataFrameTime < TimeSpan.FromSeconds(1))
                {
                    ConsoleApp.Stats.WriteLine("Bailing connection as too early!");
                    return true;
                }
                LastDataFrameTime = DateTime.UtcNow;
                _socket = new Socket(_ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                _socket.Connect(_remoteEP);

                BytesSentSinceFrame = 0;
                ConsoleApp.Stats.WriteLine("Connected to " + _remoteEP);

                return true;
            }
            catch (SocketException)
            {
                IsDead = true;
                return false;
            }
        }

        public void Close()
        {
            if (_socket != null && _socket.Connected)
            {
                _socket.Close();
                _socket = null;
            }
        }
        // SocketResponse
        //
        // The response structure sent back to us when we deliver a frame of data to the NightDriverStrip

        unsafe public uint SendData(byte[] data, ref SocketResponse response)
        {
            if (_socket == null)
                return 0;

            uint result = (uint)_socket.Send(data);
            if (result != data.Length)
            {
                IsDead = true;

                return result;
            }

            TimeSpan timeSinceLastSend = DateTime.UtcNow - LastDataFrameTime;
            if (timeSinceLastSend > TimeSpan.FromSeconds(10.0))
            {
                LastDataFrameTime = DateTime.UtcNow;
                BytesSentSinceFrame = 0;
            }
            else
            {
                BytesSentSinceFrame += result;
            }

            DateTime startWaiting = DateTime.UtcNow;
            // Receive the response back from the socket we just sent to
            int cbToRead = sizeof(SocketResponse);
            byte[] buffer = new byte[cbToRead];

            // Wait until there's enough data to process or we've waited 5 seconds with no result

            //while (DateTime.UtcNow - startWaiting > TimeSpan.FromSeconds(5) && _socket.Available < cbToRead)
            //    Thread.Sleep(100);

            while (_socket.Available >= cbToRead)
            {
                var readBytes = _socket.Receive(buffer, cbToRead, SocketFlags.None);
                if (readBytes >= sizeof(SocketResponse) && buffer[0] >= sizeof(SocketResponse))
                {
                    GCHandle pinnedArray = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    nint pointer = pinnedArray.AddrOfPinnedObject();
                    response = Marshal.PtrToStructure<SocketResponse>(pointer);
                    pinnedArray.Free();
                }
                FirmwareVersion = "v" + response.flashVersion;
            }
            return result;
        }
    }
}
