//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2023 Dave Plummer.  All Rights Reserved.
//
// File:        LEDEffect.cs
//
// Description:
//
//   Represents an effect object that knows how to Render itself
//   
// History:     Dec-18-2023        Davepl      Cleanup
//
//---------------------------------------------------------------------------
using System.Diagnostics;

namespace NightDriver
{
    public class LEDEffect
    {
        public virtual string EffectName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        protected virtual void Render(ILEDGraphics graphics)
        {
            Debug.Assert(false, "Render called in base class");
        }

        public void DrawFrame(ILEDGraphics graphics)
        {
            //lock(graphics)
            {
                Render(graphics);
            }
        }

    }
}