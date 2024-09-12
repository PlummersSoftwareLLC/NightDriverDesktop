//+--------------------------------------------------------------------------
//
// NightDriverDesktop - (c) 2024 Dave Plummer.  All Rights Reserved.
//
// File:        StripListView.cs
//
// Description:
//
//   A ListViewItem that represents a LightStrip entry in the main strip list
//
// History:     Dec-23-2023        Davepl      Created
//
//---------------------------------------------------------------------------

using System.Collections;
using System.Diagnostics;
using System.Net;

namespace NightDriver
{
    public enum StripListViewColumnIndex
    {
        INVALID = -1,
        FIRST = 1,
        iHost = 1,
        iHasSocket,
        iWiFiSignal,
        iReadyForData,
        iBytesPerSecond,
        iCurrentClock,
        iBufferPos,
        iWatts,
        iFpsDrawing,
        iTimeOffset,
        iConnects,
        iQueueDepth,
        iCurrentEffect,
        MAX = iCurrentEffect
    };

    class StripListItem : ListViewItem
    {
        public string Host
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iHost].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iHost].Text = value;
            }
        }

        public string HasSocket
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iHasSocket].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iHasSocket].Text = value;
            }
        }

        public string WiFiSignal
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iWiFiSignal].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iWiFiSignal].Text = value;
            }
        }

        public string ReadyForData
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iReadyForData].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iReadyForData].Text = value;
            }
        }

        public string BytesPerSecond
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iBytesPerSecond].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iBytesPerSecond].Text = value;
            }
        }

        public string CurrentClock
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iCurrentClock].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iCurrentClock].Text = value;
            }
        }

        public string BufferPos
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iBufferPos].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iBufferPos].Text = value;
            }
        }

        public string Watts
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iWatts].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iWatts].Text = value;
            }
        }

        public string FpsDrawing
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iFpsDrawing].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iFpsDrawing].Text = value;
            }
        }

        public string TimeOffset
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iTimeOffset].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iTimeOffset].Text = value;
            }
        }

        public string Connects
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iConnects].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iConnects].Text = value;
            }
        }

        public string QueueDepth
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iQueueDepth].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iQueueDepth].Text = value;
            }
        }

        public string CurrentEffect
        {
            get
            {
                return SubItems[(int)StripListViewColumnIndex.iCurrentEffect].Text;
            }
            set
            {
                SubItems[(int)StripListViewColumnIndex.iCurrentEffect].Text = value;
            }
        }

        public string Location
        {
            get
            {
                return Text;
            }
        }


        // UpdateColumnText

        public bool UpdateColumnText(StripListItem other, StripListViewColumnIndex idx = StripListViewColumnIndex.INVALID)
        {
            if (idx == StripListViewColumnIndex.INVALID)
            {
                bool bChanged = false;
                for (int i = 0; i < SubItems.Count; i++)
                    bChanged |= UpdateColumnText(other, (StripListViewColumnIndex)i);
                return bChanged;
            }
            else
            {
                if (SubItems[(int)idx].Text != other.SubItems[(int)idx].Text)
                {
                    SubItems[(int)idx].Text = other.SubItems[(int)idx].Text;
                    return true;
                }
                return false;
            }
        }

        public StripListItem(ListViewGroup group, String text, LightStrip? strip)
        {
            Text = text;
            Group = group;
            Tag = strip;

            if (strip != null)
                for (StripListViewColumnIndex i = 0; i < StripListViewColumnIndex.MAX; i++)
                    SubItems.Add(new ListViewItem.ListViewSubItem(this, "---"));
        }

        public static StripListItem CreateForStrip(ListViewGroup group, LightStrip strip)
        {
            double epoch = (DateTime.UtcNow.Ticks - DateTime.UnixEpoch.Ticks) / (double)TimeSpan.TicksPerSecond;
            var item = new StripListItem(group, strip.FriendlyName, strip);

            item.Host = strip.HostName;
            item.HasSocket = !strip.HasSocket ? "No" : "Open";
            item.WiFiSignal = !strip.HasSocket ? "---" : strip.Response.wifiSignal.ToString();
            item.ReadyForData = !strip.HasSocket ? "---" : strip.ReadyForData ? "Ready" : "No";
            item.BytesPerSecond = !strip.HasSocket ? "---" : strip.BytesPerSecond.ToString();
            item.CurrentClock = !strip.HasSocket ? "---" : strip.Response.currentClock > 8 ? (strip.Response.currentClock - epoch).ToString("F2") : "UNSET";
            item.BufferPos = !strip.HasSocket ? "---" : strip.Response.bufferPos.ToString() + "/" + strip.Response.bufferSize.ToString();
            item.Watts = !strip.HasSocket ? "---" : strip.Response.watts.ToString();
            item.FpsDrawing = !strip.HasSocket ? "---" : strip.Response.fpsDrawing.ToString();
            item.TimeOffset = !strip.HasSocket ? "---" : strip.TimeOffset.ToString();
            item.Connects = strip.Connects.ToString();
            item.QueueDepth = strip.QueueDepth.ToString();
            item.CurrentEffect = strip.StripSite.CurrentEffectName;
            item.Group = group;

            Debug.Assert(item.Tag == strip);

            return item;
        }
    }

    public class StripListItemComparer : IComparer
    {
        internal StripListViewColumnIndex Column;
        internal SortOrder SortOrder;

        private static long IpToLong(string ip)
        {
            if (ip == "---")
                return 0;
            return BitConverter.ToInt32(IPAddress.Parse(ip).GetAddressBytes(), 0);
        }

        public StripListItemComparer(StripListViewColumnIndex column)
        {
            this.Column = column;
            this.SortOrder = SortOrder.Ascending;
        }

        private static int CompareTextNumbers(StripListItem left, StripListItem right, StripListViewColumnIndex idx)
        {
            double leftVal, rightVal;
            if (!Double.TryParse(left.SubItems[(int)idx].Text, out leftVal))
                leftVal = 0;
            if (!Double.TryParse(right.SubItems[(int)idx].Text, out rightVal))
                rightVal = 0;
            return leftVal.CompareTo(rightVal);
        }

        public int Compare(object x, object y)
        {
            var left = x as StripListItem;
            var right = y as StripListItem;

            if (left == null || right == null)
                return 0;

            var mult = (SortOrder == SortOrder.Ascending) ? 1 : -1;

             switch (Column)
            {
                case 0:
                    return left.Location.CompareTo(right.Location) * mult;

                case StripListViewColumnIndex.iWiFiSignal:
                case StripListViewColumnIndex.iWatts:
                case StripListViewColumnIndex.iBytesPerSecond:
                case StripListViewColumnIndex.iCurrentClock:
                case StripListViewColumnIndex.iFpsDrawing:
                case StripListViewColumnIndex.iTimeOffset:
                case StripListViewColumnIndex.iConnects:
                case StripListViewColumnIndex.iQueueDepth:
                    return CompareTextNumbers(left, right, Column) * mult;

                case StripListViewColumnIndex.iHost:
                    if (left.Tag == null && right.Tag == null)                                  // Both are groups, compare by group name
                    {
                        return left.Group.Name.CompareTo(right.Group.Name) * mult;
                    }
                    if (left.Tag == null && right.Group == left.Group)
                        return -1;                                                              // Group is always above a strip that belongs to it
                    if (right.Tag == null && right.Group == left.Group)
                        return 1;                                                               // Strip is always greater than a group
                    if (left.Group == right.Group)
                    {
                        return left.Host.CompareTo(right.Host) * mult;
                    }
                    else
                    {
                        return left.Group.Name.CompareTo(right.Group.Name) * mult;
                    }
                    return 0;
  

                case StripListViewColumnIndex.iHasSocket:
                    return left.HasSocket.CompareTo(right.HasSocket) * mult;

                case StripListViewColumnIndex.iReadyForData:
                    return left.ReadyForData.CompareTo(right.ReadyForData) * mult;

                case StripListViewColumnIndex.iBufferPos:
                    return left.BufferPos.CompareTo(right.BufferPos) * mult;

                case StripListViewColumnIndex.iCurrentEffect:
                    return left.CurrentEffect.CompareTo(right.CurrentEffect) * mult;

                default:
                    return 0;
            }
        }

        public void ToggleSortOrder()
        {
            SortOrder = (SortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
        }
    }

}