//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2023 Dave Plummer.  All Rights Reserved.
//
// File:        LightStrip.cs
//
// Description:
//
// An LEDController specialization for standard strips.  Provides the ability
// to reverse layout and to modify the RGB order for this strip
//
// History:     Dec-18-2023        Davepl      Cleanup
//
//---------------------------------------------------------------------------

using Newtonsoft.Json;

namespace NightDriver
{
    [Serializable]
    public class LightStrip : LEDControllerChannel
    {
        public uint FramesPerBuffer { get; set; } = 21;                 // How many buffer frames the chips have
        public double PercentBufferUse { get; set; } = 0.85;            // How much of the buffer we should use up

        // The only attribute that a light strip adds is that it can be reversed, as you
        // could hand it from either end

        public bool Reversed { get; set; } = false;

        [JsonIgnore]
        public double TimeOffset
        {
            get
            {
                if (null == StripSite)
                    return 0;

                if (0 == StripSite.FramesPerSecond)                  // No speed indication yet, can't guess at offset, assume 1 second for now
                    return 0.0;

                double offset = FramesPerBuffer * PercentBufferUse / StripSite.FramesPerSecond;
                return offset;
            }
        }

        public LightStrip(string hostName,
                          string friendlyName,
                          bool compressData,
                          uint width,
                          uint height = 1,
                          uint offset = 0,
                          bool reversed = false,
                          byte channel = 0,
                          bool redGreenSwap = false,
                          uint batchSize = 1)
        : base(hostName,
               friendlyName,
               width,
               height,
               offset,
               compressData,
               channel,
               0,
               redGreenSwap,
               batchSize)
        {
            Reversed = reversed;
        }

        public byte[] GetPixelData(CRGB[] LEDs)
        {
            return LEDInterop.GetColorBytesAtOffset(LEDs, Offset, Width * Height, Reversed, RedGreenSwap);
        }

        const ushort WIFI_COMMAND_PIXELDATA = 0;
        const ushort WIFI_COMMAND_VU = 1;
        const ushort WIFI_COMMAND_CLOCK = 2;
        const ushort WIFI_COMMAND_PIXELDATA64 = 3;

        protected override byte[] GetDataFrame(CRGB[] MainLEDs, DateTime timeStart)
        {
            // The timeOffset is how far in the future frames are generated for.  If the chips have a 2 second buffer, you could
            // go up to 2 seconds, but I shoot for the middle of the buffer depth.  Right now it's calculated as using 

            double epoch = (timeStart.Ticks - DateTime.UnixEpoch.Ticks + TimeOffset * TimeSpan.TicksPerSecond) / TimeSpan.TicksPerSecond;

            ulong seconds = (ulong)epoch;                                       // Whole part of time number (left of the decimal point)
            ulong uSeconds = (ulong)((epoch - (int)epoch) * 1000000);           // Fractional part of time (right of the decimal point)

            var data = GetPixelData(MainLEDs);
            return LEDInterop.CombineByteArrays(LEDInterop.WORDToBytes(WIFI_COMMAND_PIXELDATA64),      // Offset, always zero for us
                                                LEDInterop.WORDToBytes(Channel),               // LED channel on ESP32
                                                LEDInterop.DWORDToBytes((uint)data.Length / 3),      // Number of LEDs
                                                LEDInterop.ULONGToBytes(seconds),                      // Timestamp seconds (64 bit)
                                                LEDInterop.ULONGToBytes(uSeconds),                     // Timestmap microseconds (64 bit)
                                                data);                                                 // Color Data

        }
    };
}

