using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;


namespace NightDriver
{
    public class TextBitmapEffect : LEDEffect
    {
        private Random rand = new Random();
        private List<Star> stars = new List<Star>();

        // Star class to hold position, speed, and color
        private class Star
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Speed { get; set; }
            public Color StarColor { get; set; }

            public Star(float x, float y, float speed, Color color)
            {
                X = x;
                Y = y;
                Speed = speed;
                StarColor = color;
            }
        }

        // Initialize stars with random positions, speeds, and colors
        private void InitializeStars(int starCount, int width, int height)
        {
            stars.Clear();
            for (int i = 0; i < starCount; i++)
            {
                float x = rand.Next(width);
                float y = rand.Next(height);
                float speed = (float)(rand.NextDouble() * 0.025 + 0.005); // Slowed down speed by 50%
                Color color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)); // Random color
                stars.Add(new Star(x, y, speed, color));
            }
        }

        // Draw the starfield effect with random colors
        public void Starfield(Graphics graphics, int width, int height)
        {
            if (stars.Count == 0)
                InitializeStars(250, width, height); // Increased star count by 25%

            foreach (var star in stars)
            {
                // Move the star towards the center
                star.X += (star.X - width / 2) * star.Speed;
                star.Y += (star.Y - height / 2) * star.Speed;

                // If the star goes out of bounds, reset its position
                if (star.X < 0 || star.X >= width || star.Y < 0 || star.Y >= height)
                {
                    star.X = rand.Next(width);
                    star.Y = rand.Next(height);
                    star.Speed = (float)(rand.NextDouble() * 0.005 + 0.005); // New speed, slowed by 50%
                    star.StarColor = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256)); // New random color
                }

                // Draw the star in its random color
                using (Brush brush = new SolidBrush(star.StarColor))
                {
                    graphics.FillRectangle(brush, star.X, star.Y, 1, 1);
                }
            }
        }

        private string _text;
        private CRGB _backgroundColor;
        private CRGB _textColor;

        public TextBitmapEffect(string text, CRGB backgroundColor, CRGB textColor)
        {
            _text = text;
            _backgroundColor = backgroundColor;
            _textColor = textColor;
        }

        protected override void Render(ILEDGraphics graphics)
        {
            // Define font size based on the LED matrix height
            uint fontSize = (uint)(Math.Min(graphics.Width, graphics.Height) / 1.5); // Adjust font size as needed

            // Create a bitmap buffer for the text
            using (var bitmap = new Bitmap((int)graphics.Width, (int)graphics.Height))
            {
                using (var graphicsBitmap = Graphics.FromImage(bitmap))
                {
                    graphicsBitmap.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    // Fill the bitmap with the background color
                    graphicsBitmap.Clear(Color.FromArgb(_backgroundColor.r, _backgroundColor.g, _backgroundColor.b));

                    Starfield(graphicsBitmap, (int)graphics.Width, (int)graphics.Height); // Draw the Starfield effect

                    // Create a font and brush for drawing textF
                    var font = new Font("Arial", fontSize, FontStyle.Regular);
                    var brush = new SolidBrush(Color.FromArgb(_textColor.r, _textColor.g, _textColor.b));

                    // Calculate text position
                    var textSize = graphicsBitmap.MeasureString(_text, font);
                    var x = (graphics.Width - textSize.Width) / 2;
                    var y = (graphics.Height - textSize.Height) / 2;

                    // Draw text on the bitmap
                    graphicsBitmap.DrawString(_text, font, brush, x, y);
                }

                // Render the bitmap onto the LED display
                for (uint x = 0; x < graphics.Width; x++)
                {
                    for (uint y = 0; y < graphics.Height; y++)
                    {
                        // Get pixel color from the bitmap
                        var pixelColor = bitmap.GetPixel((int)x, (int)y);
                        graphics.DrawPixel(x, y, new CRGB(pixelColor.R, pixelColor.G, pixelColor.B));
                    }
                }
            }
        }
    }

    public class SimpleColorFillEffect : LEDEffect
    {
        // Update is called once per frame

        public CRGB _color;
        public uint _everyNth;

        public SimpleColorFillEffect(CRGB color, uint everyNth = 10)
        {
            _everyNth = everyNth;
            _color = color;
        }

        protected override void Render(ILEDGraphics graphics)
        {
            graphics.FillSolid(CRGB.Black);
            for (uint i = 0; i < graphics.DotCount; i += _everyNth)
                graphics.DrawPixel(i, _color);
        }
    };


    public class RainbowEffect : LEDEffect
    {
        // Update is called once per frame
        public double _deltaHue;
        public double _startHue;
        public double _hueSpeed;

        DateTime _lastDraw = DateTime.UtcNow;

        public RainbowEffect(double deltaHue = 0, double hueSpeed = 5)
        {
            _deltaHue = deltaHue;
            _startHue = 0;
            _hueSpeed = hueSpeed;
        }

        protected override void Render(ILEDGraphics graphics)
        {
            double delta = _hueSpeed * (double)(DateTime.UtcNow - _lastDraw).TotalSeconds;
            _lastDraw = DateTime.UtcNow;
            _startHue = _startHue + delta;

            // BUGBUG It stymies me as to why one is modulus 360 and the other is 256! (davepl)

            CRGB color = CRGB.HSV2RGB(_startHue % 360);
            if (_deltaHue == 0.0)
            {
                graphics.FillSolid(color);
            }
            else
            {
                graphics.FillRainbow(_startHue % 360, _deltaHue);
                graphics.Blur(3);
            }
            //Console.WriteLine(delta.ToString() + ", : " + _startHue.ToString() + "r = " + color.r + " g = " + color.g + " b = " + color.b);
        }
    };


    public class TestEffect : LEDEffect
    {
        private uint _startIndex;
        private uint _length;
        private CRGB _color;

        public TestEffect(uint startIndex, uint length, CRGB color)
        {
            _startIndex = startIndex;
            _length = length;
            _color = color;
        }

        DateTime _lastDraw = DateTime.UtcNow;

        // Update is called once per frame

        protected override void Render(ILEDGraphics graphics)
        {
            for (uint i = _startIndex; i < _startIndex + _length; i++)
                graphics.DrawPixel(i, _color);

            graphics.DrawPixel(_startIndex, CRGB.White);
            graphics.DrawPixel(_startIndex + 1, CRGB.Black);
            graphics.DrawPixel(_startIndex + _length - 1, CRGB.White);
            graphics.DrawPixel(_startIndex + _length - 2, CRGB.Black);

        }
    }




}