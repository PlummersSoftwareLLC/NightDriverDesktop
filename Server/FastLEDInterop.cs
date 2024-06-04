//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2019 Dave Plummer.  All Rights Reserved.
//
// File:        LEDInterop.cs
//
// Description:
//
// Functions for working directly with the color data in an LED string and
// to compress and uncompress data from the color array
//
// History:     Dec-18-2023        Davepl      Cleanup
//
//---------------------------------------------------------------------------


using System.Collections.Concurrent;
using System.Diagnostics;
using ZLIB;

namespace NightDriver
{
    public static class MathExtensions
    {
        public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }

    public static class QueueExtensions
    {
        public static IEnumerable<T> DequeueChunk<T>(this ConcurrentQueue<T> queue, int chunkSize)
        {
            for (int i = 0; i < chunkSize && queue.Count > 0; i++)
            {
                T result;
                if (false == queue.TryDequeue(out result))
                    throw new Exception("Unable to Dequeue the data!");
                yield return result;
            }
        }
    }

    public static class LEDInterop
    {
        // scale8 - Given a value i, scales it down by scale/256th 

        public static byte scale8(byte i, byte scale)
        {
            return (byte)(((ushort)i * (ushort)(scale)) >> 8);
        }

        public static byte scale8_video(byte i, byte scale)
        {
            byte j = (byte)((((int)i * (int)scale) >> 8) + ((i != 0 && scale != 0) ? 1 : 0));
            return j;
        }

        // fill_solid - fills a rnage of LEDs with a given color value

        public static void fill_solid(CRGB[] leds, CRGB color)
        {
            for (int i = 0; i < leds.Length; i++)
                leds[i] = color;
        }

        // fill_rainbow - fills a range of LEDs rotating through a hue wheel

        public static void fill_rainbow(CRGB[] leds, byte initialHue, double deltaHue)
        {
            double hue = initialHue;
            for (int i = 0; i < leds.Length; i++, hue += deltaHue)
                leds[i] = CRGB.HSV2RGB(hue % 360, 1.0, 1.0);
        }

        // GetColorBytesAtOffset
        //
        // Reach into the main array of CRGBs and grab the color bytes for a strand

        public static byte [] GetColorBytesAtOffset(CRGB [] main, uint offset, uint length, bool bReversed = false, bool bRedGreenSwap = false)
        {
            byte[] data = new byte[length * 3];
            for (int i = 0; i < length; i++)
            {
                if (bRedGreenSwap)
                {
                    data[i * 3]     = bReversed ? main[offset + length - 1 - i].r : main[offset + i].g;
                    data[i * 3 + 1] = bReversed ? main[offset + length - 1 - i].g : main[offset + i].r;
                }
                else
                {
                    data[i * 3]     = bReversed ? main[offset + length - 1 - i].r : main[offset + i].r;
                    data[i * 3 + 1] = bReversed ? main[offset + length - 1 - i].g : main[offset + i].g;
                }
                data[i * 3 + 2] = bReversed ? main[offset + length - 1 - i].b : main[offset+i].b;
            }
            return data;
        }

        // GetColorsFromBytes
        //
        // Given a series of bytes in memory, returns them as an array of CRGB objects. 

        public static CRGB[] GetColorsFromBytes(byte[] data, uint length)
        {
            CRGB[] colors = new CRGB[length];
            for (int i = 0; i < length; i++)
            {
                colors[i] = new CRGB
                {
                    r = data[i * 3],
                    g = data[i * 3 + 1],
                    b = data[i * 3 + 2]
                };
            }
            return colors;
        }

        public static byte[] ULONGToBytes(UInt64 input)
        {
            return new byte[8]
            {
                (byte)((input      ) & 0xff),
                (byte)((input >>  8) & 0xff),
                (byte)((input >> 16) & 0xff),
                (byte)((input >> 24) & 0xff),
                (byte)((input >> 32) & 0xff),
                (byte)((input >> 40) & 0xff),
                (byte)((input >> 48) & 0xff),
                (byte)((input >> 56) & 0xff),
            };
        }

        public static byte[] DWORDToBytes(UInt32 input)
        {
            return new byte[4]
            {
                (byte)((input      ) & 0xff),
                (byte)((input >>  8) & 0xff),
                (byte)((input >> 16) & 0xff),
                (byte)((input >> 24) & 0xff),
            };
        }

        public static byte[] WORDToBytes(UInt16 input)
        {
            return new byte[2]
            {
                (byte)((input      ) & 0xff),
                (byte)((input >>  8) & 0xff),
            };
        }

        public static UInt64 BytesToULONG(byte[] bytes, int startIndex = 0)
        {
            return (UInt64)bytes[startIndex] |
                   ((UInt64)bytes[startIndex + 1] << 8) |
                   ((UInt64)bytes[startIndex + 2] << 16) |
                   ((UInt64)bytes[startIndex + 3] << 24) |
                   ((UInt64)bytes[startIndex + 4] << 32) |
                   ((UInt64)bytes[startIndex + 5] << 40) |
                   ((UInt64)bytes[startIndex + 6] << 48) |
                   ((UInt64)bytes[startIndex + 7] << 56);
        }

        public static UInt32 BytesToDWORD(byte[] bytes, int startIndex = 0)
        {
            return (UInt32)bytes[startIndex] |
                   ((UInt32)bytes[startIndex + 1] << 8) |
                   ((UInt32)bytes[startIndex + 2] << 16) |
                   ((UInt32)bytes[startIndex + 3] << 24);
        }

        // CombineByteArrays - Combine N arrays and returns them as one new big new one

        public static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        // CompressMemory
        //
        // Compress a buffer using ZLIB, return the compressed version of it as a ZLIB stream

        public static byte[] Compress(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new ZLIBStream(compressedStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        // DecompressMemory
        //
        // Expands a buffer using ZLib, returns the uncompressed version of it

        public static byte[] Decompress(byte[] input)
        {
            using (var inStream = new MemoryStream(input))
            using (var bigStream = new ZLIBStream(inStream, System.IO.Compression.CompressionMode.Decompress))
            using (var bigStreamOut = new MemoryStream())
            {
                bigStream.CopyTo(bigStreamOut);
                return bigStreamOut.ToArray();
            }
        }
    }
}

