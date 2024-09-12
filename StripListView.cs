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
                e.Graphics.FillRectangle(Brushes.LightSlateGray, e.Bounds);
            }
            else
            {
                // Draw default background
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            // Let the default handler draw the rest of the items
            e.DrawDefault = false;
        }

        private void StripListView_MouseClick(object sender, MouseEventArgs e)
        {
            // Determine if a group header was clicked
            foreach (ListViewGroup group in this.Groups)
            {
                // Find the bounds of the first item in each group
                if (group.Items.Count > 0)
                {
                    ListViewItem firstItem = group.Items[0];
                    Rectangle itemBounds = firstItem.GetBounds(ItemBoundsPortion.Entire);
                    Rectangle groupHeaderBounds = new Rectangle(itemBounds.Left, itemBounds.Top - this.Font.Height - 6, this.Width, this.Font.Height + 8);

                    // Check if the mouse click is within the bounds of the group header
                    if (groupHeaderBounds.Contains(e.Location))
                    {
                        _selectedGroup = group; // Set the clicked group as selected
                        this.Invalidate(); // Force the control to redraw
                        return;
                    }
                }
            }

            _selectedGroup = null; // Clear selection if no group header is clicked
            this.Invalidate();
        }

        // Handle drawing of sub-items (default handling)
        private void StripListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.Item.Tag == null)
            {
                // Custom drawing for items with null Tag
                e.Graphics.FillRectangle(e.Item.Selected ? Brushes.Black : Brushes.CornflowerBlue, e.Bounds);
                // Create a bold font based on the control's font
                using (Font boldFont = new Font(e.SubItem.Font, FontStyle.Bold))
                {
                    // Draw the text in white using the bold font
                    e.Graphics.DrawString(e.SubItem.Text, boldFont, e.Item.Selected ? Brushes.White : Brushes.Black, e.Bounds);
                }
            }
            else
            {
                var bounds = e.Bounds;
                // Default drawing for other items
                if (e.ColumnIndex == 0)
                    bounds.X += 10;
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
