using FFMediaToolkit;
using FFMediaToolkit.Decoding;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Reflection;

namespace NightDriver
{
    public unsafe class VideoPlayerEffect : LEDEffect
    {
        private readonly string _videoFilePath;
        private MediaFile? _mediaFile;
        private readonly Image<Rgb24> _resizedFrame;

        public VideoPlayerEffect(string videoFilePath)
        {
            if (!Environment.Is64BitProcess)
                throw new InvalidOperationException("This effect must be part of a 64-bit executable.");

            _videoFilePath = videoFilePath;
            FFmpegLoader.FFmpegPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "native", "ffmpeg");
            _resizedFrame = new Image<Rgb24>(512, 32);
        }

        ~VideoPlayerEffect()
        {
            _mediaFile?.Dispose();
            _resizedFrame.Dispose();
        }

        public override void OnStart()
        {
            try
            {
                _mediaFile = MediaFile.Open(_videoFilePath);
            }
            catch (Exception)
            {
                MessageBox.Show("Can't load video: " + _videoFilePath);
                throw;
            }
        }

        public override void OnStop()
        {
            _mediaFile?.Dispose();
        }

        protected override void Render(ILEDGraphics graphics)
        {
            if (_mediaFile?.Video.TryGetNextFrame(out var frame) ?? false)
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
                _mediaFile?.Dispose();
                _mediaFile = MediaFile.Open(_videoFilePath);
            }
        }
    }
}