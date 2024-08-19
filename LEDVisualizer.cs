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


namespace NightDriver
{
    internal class LEDVisualizer : Panel
    {
        uint xSpacing = 1;
        uint ySpacing = 1;
        uint xMargin = 4;
        uint yMargin = 4;
        uint xSize = 8;
        uint ySize = 8;

        // CalculateMaxSquareSize
        //
        // Figures out the largest square that can be used to represent this strip in the available
        // client area

        private void CalculateMaxSquareSize()
        {
            if (ColorData == null || ColorData.Length == 0)
                return;

            uint totalSquares = (uint)ColorData.Length;
            uint availableWidth = (uint)ClientRectangle.Width - xMargin * 2;
            uint availableHeight = (uint)ClientRectangle.Height - yMargin * 2;

            if (fixedWidth > 0)
            {
                // Calculate the size of each square based on the fixed number of columns
                xSize = (availableWidth - (fixedWidth - 1) * xSpacing) / fixedWidth;

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
                uint squaresPerRow = (availableWidth + xSpacing) / (maxSize + xSpacing);
                uint squaresPerColumn = (availableHeight + ySpacing) / (maxSize + ySpacing);

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
            uint availableXSlots = fixedWidth > 0 ? fixedWidth : availableWidth / (xSize + xSpacing);
            uint availableHeight = (uint)ClientRectangle.Height - yMargin * 2;
            uint availableYSlots = availableHeight / (ySize + ySpacing);

            // The Draw thread will also lock the buffer, and that synchronization allows us to 
            // avoid showing a frame for visualization when the buffer is halfway through a render

            lock (ColorData)
            {
                uint iSlot = 0;
                for (uint y = 0; y < availableYSlots; y++)
                    for (uint x = 0; x < availableXSlots; x++)
                        if (iSlot < ColorData.Length)
                            e.Graphics.FillRectangle(new SolidBrush(ColorData[iSlot++].GetColor()),
                                                     xMargin + x * (xSize + xSpacing),
                                                     yMargin + y * (ySize + ySpacing),
                                                     xSize,
                                                     ySize);
            }
        }
    }
}
