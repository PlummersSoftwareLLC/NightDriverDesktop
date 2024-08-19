using FFmpeg.AutoGen;
using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Runtime.InteropServices;

// winget install "FFmpeg (Shared)"

namespace NightDriver
{
    public unsafe class VideoPlayerEffect : LEDEffect
    {
        private string _videoFilePath;
        private MediaFile _mediaFile;
        private Image<Rgb24> _resizedFrame;

        public VideoPlayerEffect(string videoFilePath)
        {
            _videoFilePath = videoFilePath;
            FFmpegLoader.FFmpegPath = @"C:\Users\dave\AppData\Local\Microsoft\WinGet\Packages\Gyan.FFmpeg.Shared_Microsoft.Winget.Source_8wekyb3d8bbwe\ffmpeg-7.0.2-full_build-shared\bin\";
            _mediaFile = MediaFile.Open(_videoFilePath);
            _resizedFrame = new Image<Rgb24>(512, 32);
        }

        ~VideoPlayerEffect()
        {
            _mediaFile?.Dispose();
            _resizedFrame?.Dispose();
        }

        protected override void Render(ILEDGraphics graphics)
        {
            if (_mediaFile.Video.TryGetNextFrame(out var frame))
            {
                using (var sourceImage = SixLabors.ImageSharp.Image.LoadPixelData<Bgr24>(
                    frame.Data,
                    frame.ImageSize.Width,
                    frame.ImageSize.Height))
                {
                    sourceImage.Mutate(x => x
                        .Resize(new SixLabors.ImageSharp.Size((int)graphics.Width, (int)graphics.Height)));

                    // Convert to Rgb24 and copy to _resizedFrame
                    sourceImage.CloneAs<Rgb24>().ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgb24> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < pixelRow.Length; x++)
                            {
                                _resizedFrame[x, y] = pixelRow[x];
                            }
                        }
                    });
                }

                // Iterate through pixels and draw them
                for (int y = 0; y < graphics.Height; y++)
                {
                    for (int x = 0; x < graphics.Width; x++)
                    {
                        var pixel = _resizedFrame[x, y];
                        graphics.DrawPixel((uint)x, (uint)y, new CRGB(pixel.R, pixel.G, pixel.B));
                    }
                }
            }
            else
            {
                // End of video, reset to start. I'd rather seek but can't figure out how!
                _mediaFile.Dispose();
                _mediaFile = MediaFile.Open(_videoFilePath);
            }
        }
    }
}