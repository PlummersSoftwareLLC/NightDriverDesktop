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

using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using static System.Windows.Forms.AxHost;

namespace NightDriver
{
    internal class StripListView : ListView
    {
        private ListViewGroup _selectedGroup; // Field to store the selected group

        public StripListView()
        {
            // Activate double buffering
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Enable notify messages so we get a shot at WM_ERASEBKGND
            SetStyle(ControlStyles.EnableNotifyMessage, true);

            // Attach event handlers for custom drawing
            this.OwnerDraw = true;
            this.DrawItem += StripListView_DrawItem;
            this.DrawSubItem += StripListView_DrawSubItem;
            this.DrawColumnHeader += StripListView_DrawColumnHeader;
            this.MouseClick += StripListView_MouseClick;
        }

        private void StripListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            StripListView stripListView = (StripListView)sender;

            // Draw the background of the item, handling selection state
            if (e.Item.Selected)
            {
                // Draw selected background
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(192,192,192)), e.Bounds);
            }
            else
            {
                // Draw default background
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(220, 220, 220)), e.Bounds);
            }
            // Let the default handler draw the rest of the items
            e.DrawDefault = false;
        }

        private void StripListView_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the item at the mouse click location
            var hitTestInfo = HitTest(e.Location);
            if (hitTestInfo.Item != null && hitTestInfo.SubItem != null)
            {
                // Get the bounds of the first subitem (checkbox column)
                Rectangle subItemBounds = hitTestInfo.SubItem.Bounds;

                // Define the checkbox bounds relative to the subitem bounds
                Rectangle checkBounds = new Rectangle(subItemBounds.Left + 4, subItemBounds.Top + 4, 16, 16);

                // Check if the click is within the checkbox bounds (translated into control coordinates)
                if (hitTestInfo.Item.Tag == null && hitTestInfo.Item.SubItems.IndexOf(hitTestInfo.SubItem) == 0 && checkBounds.Contains(e.Location))
                {
                    // Toggle the checked state
                    hitTestInfo.Item.Checked = !hitTestInfo.Item.Checked;

                    // Redraw the item to reflect the checkbox state change
                    Invalidate(hitTestInfo.SubItem.Bounds);

                    // Raise item changed event
                    OnItemChecked(new ItemCheckedEventArgs(hitTestInfo.Item));
                }
            }
        }

        // Handle drawing of sub-items (default handling)
        private void StripListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Item.Tag == null)
            {
                // Custom drawing for items with null Tag
                e.Graphics.FillRectangle(e.Item.Selected ? Brushes.Black : new SolidBrush(Color.FromArgb(150, 190, 255)), e.Bounds);

                // Draw the checkbox
                if (e.ColumnIndex == 0)
                {
                    Rectangle bounds = e.Bounds;
                    CheckBoxRenderer.DrawCheckBox(e.Graphics,
                                                  new Point(bounds.Left + 4, bounds.Top + 4),
                                                  e.Item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedNormal);
                }

                // Create a bold font based on the control's font
                using (Font boldFont = new Font(e.SubItem.Font, FontStyle.Bold))
                {
                    // Draw the text in white using the bold font
                    Rectangle bounds = e.Bounds;
                    bounds.X += 18;
                    bounds.Y += 2;
                    e.Graphics.DrawString(e.SubItem.Text, boldFont, e.Item.Selected ? Brushes.White : Brushes.Black, bounds);
                }
            }
            else
            {
                var bounds = e.Bounds;
                bounds.Y += 2;
                // Default drawing for other items
                if (e.ColumnIndex == 0)
                    bounds.X += 18;
                e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, Brushes.Black, bounds);
            }
        }

        // Handle drawing of column headers (default handling)
        private void StripListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true; // Defer to default drawing
        }

        protected override void OnNotifyMessage(Message m)
        {
            // Eat the WM_ERASEBKGND message for cleaner painting with less flicker, like Task Manager :-)
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }
    }
}
