//+--------------------------------------------------------------------------
//
// NightDriverDesktop - (c) 2024 Dave Plummer.  All Rights Reserved.
//
// File:        StripListView.cs
//
// Description:
//
//   Specialization of the listview for use in showing the main strip list.  
//
// History:     Dec-23-2023        Davepl      Created
//
//---------------------------------------------------------------------------

namespace NightDriver
{
    internal class StripListView : ListView
    {
        public StripListView()
        {
            // Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable notify messages so we get a shot at WM_ERASEBKGND
            SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        protected override void OnNotifyMessage(Message m)
        {
            // Eat the WM_ERASEBKGND message for cleaner painting with less flicker, like Task Manager :-)

            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }
    }
}