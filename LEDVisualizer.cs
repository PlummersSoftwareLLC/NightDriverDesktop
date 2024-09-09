//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2019 Dave Plummer.  All Rights Reserved.
//
// File:        LEDVisualizer.cs
//
// Description:
//
//   A custom control that renders the color data from a strip in a visual
//   window, with squares representing the LEDs in the strip.
//
// History:     Dec-23-2023        Davepl      Created
//
//---------------------------------------------------------------------------


using FFmpeg.AutoGen;

namespace NightDriver
{
    internal class LEDVisualizer : Panel
    {
        double xSpacing = 1;
        double ySpacing = 1;
        uint xMargin = 4;
        uint yMargin = 4;
        double xSize = 8;
        double ySize = 8;

        // CalculateMaxSquareSize
        //
        // Figures out the largest square that can be used to represent this strip in the available
        // client area

        private void CalculateMaxSquareSize()
        {
            if (ColorData == null || ColorData.Length == 0)
                return;

            uint totalSquares = (uint)ColorData.Length;
            var availableWidth = (uint)ClientRectangle.Width - xMargin * 2;
            var availableHeight = (uint)ClientRectangle.Height - yMargin * 2;

            if (fixedWidth > 0)
            {
                xSpacing = ySpacing = 0;
                uint height = totalSquares / fixedWidth;

                // Calculate the size of each square based on the fixed number of columns
                xSize = (int)(availableWidth - (fixedWidth - 1) * xSpacing) / fixedWidth;
                if (xSize < 1)
                    xSize = 1;

                // Calculate the number of rows needed based on the total number of squares and fixed width
                uint numRows = (totalSquares + fixedWidth - 1) / fixedWidth;

                // Calculate the maximum ySize that fits within the available height
                ySize = (availableHeight - (numRows - 1) * ySpacing) / numRows;

                // Ensure ySize is not larger than xSize
                if (ySize > xSize)
                    ySize = xSize;

                // Ensure ySize is at least 1
                if (ySize <= 0)
                    ySize = 1;

                // xSize and ySize are now calculated
                return;
            }

            // Start with a size guess
            uint maxSize = Math.Min(availableWidth, availableHeight);

            while (maxSize > 0)
            {
                uint squaresPerRow = (uint) ((availableWidth + xSpacing) / (maxSize + xSpacing));
                uint squaresPerColumn = (uint) ((availableHeight + ySpacing) / (maxSize + ySpacing));

                if (squaresPerRow * squaresPerColumn >= totalSquares)
                {
                    // Found a suitable size
                    xSize = ySize = maxSize;
                    return;
                }

                maxSize--; // Decrease size guess and try again
            }
        }

        public LEDVisualizer()
        {
            // Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        private CRGB[] _ColorData;
        public CRGB[] ColorData
        {
            get
            {
                return _ColorData;
            }
            set
            {
                _ColorData = value;
                Invalidate();
            }
        }

        private uint _fixedWidth;
        internal uint fixedWidth
        {
            get
            {
                return _fixedWidth;
            }
            set
            {
                _fixedWidth = value;
                CalculateMaxSquareSize();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            CalculateMaxSquareSize();
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (fixedWidth == 0)
            {
                xSize = Math.Clamp(xSize + (uint)e.Delta / 120, 1, 128);
                ySize = Math.Clamp(ySize + (uint)e.Delta / 120, 1, 128);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(26, 26, 26));

            if (ColorData == null)
            {
                e.Graphics.DrawString("Select an active strip for visualization", Font, Brushes.White, 2, 2);
                return;
            }

            uint availableWidth = (uint)ClientRectangle.Width - xMargin * 2;
            uint availableXSlots = (uint)(fixedWidth > 0 ? fixedWidth : availableWidth / (xSize + xSpacing));
            uint availableHeight = (uint)ClientRectangle.Height - yMargin * 2;
            uint availableYSlots = (uint)(availableHeight / (ySize + ySpacing));

            // The Draw thread will also lock the buffer, and that synchronization allows us to 
            // avoid showing a frame for visualization when the buffer is halfway through a render

            lock (ColorData)
            {
                int totalSlots = (int)(availableYSlots * availableXSlots);
                int fixedWidth = (int)availableXSlots;

                var rectangles = new List<(Rectangle Rect, Color Color)>();

                Parallel.For(0, totalSlots, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, iSlot =>
                {
                    if (iSlot < ColorData.Length)
                    {
                        int y = iSlot / fixedWidth;
                        int x = iSlot % fixedWidth;

                        int xPos = (int)(xMargin + x * (xSize + xSpacing));
                        int yPos = (int)(yMargin + ((ColorData.Length / fixedWidth) - 1 - y) * (ySize + ySpacing));

                        var rect = new Rectangle(xPos, yPos, (int)xSize, (int)ySize);
                        var color = ColorData[iSlot].GetColor();

                        lock (rectangles)
                        {
                            rectangles.Add((rect, color));
                        }
                    }
                });

                // Now draw all rectangles on the UI thread
                foreach (var (rect, color) in rectangles)
                {
                    e.Graphics.FillRectangle(new SolidBrush(color), rect);
                }
            }
        }
    }
}
