//+--------------------------------------------------------------------------
//
// NightDriver.Net - (c) 2019 Dave Plummer.  All Rights Reserved.
//
// File:        MainForm.cs
//
// Description:
//
//   The main WinForms app window, which in turn contains a tab control, 
//   and those tabs contains the main UI for the app as well as logging etc.
//
// History:     Dec-23-2023        Davepl      Created
//
//---------------------------------------------------------------------------



using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NightDriver
{
    public partial class MainForm : Form, IMessageFilter
    {
        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                DWMWINDOWATTRIBUTE attribute,
                                                ref int pvAttribute,
                                                uint cbAttribute);

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        LEDServer _server = new LEDServer();

        private Task backgroundTask;
        private CancellationTokenSource cancellationTokenSource;


        // Restore window layout
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Restore window size, location, and state
            if (ndd2.Properties.Settings.Default.WindowSize != Size.Empty)
            {
                this.Size = ndd2.Properties.Settings.Default.WindowSize;
            }

            if (ndd2.Properties.Settings.Default.WindowLocation != Point.Empty)
            {
                // Validate that the location is within screen bounds
                Rectangle screenBounds = Screen.FromPoint(ndd2.Properties.Settings.Default.WindowLocation).WorkingArea;
                Point location = ndd2.Properties.Settings.Default.WindowLocation;

                // Adjust if off-screen
                if (!screenBounds.Contains(new Rectangle(location, this.Size)))
                {
                    location = new Point(
                        Math.Max(screenBounds.Left, Math.Min(location.X, screenBounds.Right - this.Width)),
                        Math.Max(screenBounds.Top, Math.Min(location.Y, screenBounds.Bottom - this.Height))
                    );
                }

                this.Location = location;
            }

            // Restore splitter pos
            if (ndd2.Properties.Settings.Default.SplitterPos != 0)
            {
                splitContainer1.SplitterDistance = ndd2.Properties.Settings.Default.SplitterPos;
            }
        }

        // Save Window layout on close

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Persist window size, location, and state if normal or maximized (not minimized)
            if (this.WindowState == FormWindowState.Normal || this.WindowState == FormWindowState.Maximized)
            {
                ndd2.Properties.Settings.Default.WindowSize = this.Size;
                ndd2.Properties.Settings.Default.WindowLocation = this.Location;
            }
            else
            {
                // Handle minimized state (save as normal)
                ndd2.Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
                ndd2.Properties.Settings.Default.WindowLocation = this.RestoreBounds.Location;
            }

            // Save Splitter Pos
            ndd2.Properties.Settings.Default.SplitterPos = splitContainer1.SplitterDistance;

            // Save settings
            ndd2.Properties.Settings.Default.Save();
        }
        // When the user clicks the Start button we start the server

        private void StartButton_Click(object sender, EventArgs e)
        {
            ConsoleApp.Stats.WriteLine("Start Server");
            StartButton.Enabled = false;
            cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;

            backgroundTask = Task.Run(() =>
            {
                _server.Start(token);
            }, token);
            StopButton.Enabled = true;

            FillListView();
        }

        // When the user clicks the Stop button we stop the server

        private void StopButton_Click(object sender, EventArgs e)
        {
            ConsoleApp.Stats.WriteLine("Stop Server");
            _server.Stop(cancellationTokenSource);
            StartButton.Enabled = true;
            StopButton.Enabled = false;
        }

        // When the user clicks the New Strip button we populate the main list and
        // start the update timers

        public MainForm()
        {
            Application.AddMessageFilter(this);

            var preference = Convert.ToInt32(true);
            DwmSetWindowAttribute(this.Handle,
                                  DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                                  ref preference, sizeof(uint));

            InitializeComponent();

            monitorWorker.DoWork += monitorWorker_DoWork;
            monitorWorker.RunWorkerCompleted += monitorWorker_RunWorkerCompleted;

            FillListView();
            timerListView.Start();
            timerVisualizer.Start();

            UpdateUIStates();
        }

        private void tabControl_DrawItem(object? sender, DrawItemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateListView();
            toolStripStatusLabel1.Text = _server.AllStrips.Count().ToString() + " Strips, " + Process.GetCurrentProcess().Threads.Count.ToString() + " Threads";
            textLog.Text = ConsoleApp.Stats.Text;
        }

        // To fill the list view, we clear it and then iterate over all the strips
        // in all of the sites and then add them to the listview

        internal void FillListView()
        {
            stripList.Groups.Clear();
            stripList.Items.Clear();

            foreach (var strip in _server.AllStrips)
            {
                var name = strip.StripSite.Name;
                ListViewGroup group = stripList.Groups[name];
                if (group == null)
                {
                    group = stripList.Groups.Add(name, name);
                    stripList.Items.Add(new StripListItem(group, name, null, _server.AllSites[name].Enabled));
                }
                stripList.Items.Add(new StripListItem(group, strip.FriendlyName, strip));
            }
            if (stripList.Items.Count > 0)
                stripList.Items[0].Selected = true;
            UpdateUIStates();
        }

        // To update the list view we iterate over all the strips and update the details and
        // then sort this li st

        internal void UpdateListView()
        {
            foreach (var strip in _server.AllStrips)
            {
                StripListItem item = stripList.Items.OfType<StripListItem>().Where(item => item.Tag == strip).FirstOrDefault();
                var newItem = StripListItem.CreateForStrip(null, strip);
                if (item.UpdateColumnText(newItem))
                    stripList.Invalidate();
            }
            var comparer = stripList.ListViewItemSorter as StripListItemComparer;
            if (comparer == null)
                stripList.ListViewItemSorter = new StripListItemComparer(StripListViewColumnIndex.FIRST);
            stripList.Sort();
            UpdateUIStates();
        }

        // When the user clicks on a new or different entry in the list, we update the visualizer

        private void stripList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stripList.SelectedIndices.Count > 0)
            {
                var strip = stripList.SelectedItems[0].Tag as LightStrip;
                if (strip != null)
                {
                    panelVisualizer.ColorData = strip.StripSite.LEDs;
                    panelVisualizer.fixedWidth = strip.StripSite.Width > 1 ? strip.StripSite.Width : 0;

                    visualizerColorData.fixedWidth = strip.StripSite.Height > 1 ? strip.StripSite.Width : 0;
                    timerVisualizer.Interval = Math.Clamp(1000 / strip.StripSite.FramesPerSecond, 50, 500);
                }
            }
            UpdateUIStates();
        }

        // When the timer fires we invalidate the visualizer to force it to redraw

        private void timerVisualizer_Tick(object sender, EventArgs e)
        {
            panelVisualizer.Invalidate();
        }

        // Change or set the sort column

        private void stripList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var comparer = stripList.ListViewItemSorter as StripListItemComparer;
            if (comparer != null && comparer.Column == (StripListViewColumnIndex)e.Column)
                comparer.ToggleSortOrder();
            else
                stripList.ListViewItemSorter = new StripListItemComparer((StripListViewColumnIndex)e.Column);

            stripList.Sort();
        }

        // Update all of the button states based on the current state of the listview and selection

        private void UpdateUIStates()
        {
            buttonDeleteStrip.Enabled = !_server.IsRunning && stripList.SelectedIndices.Count > 0;
            buttonEditStrip.Enabled = !_server.IsRunning && stripList.SelectedIndices.Count == 1;
            buttonNewStrip.Enabled = !_server.IsRunning;

            buttonStartMonitor.Enabled = !monitorWorker.IsBusy;
            buttonStopMonitor.Enabled = monitorWorker.IsBusy;

            buttonNextEffect.Enabled = _server.IsRunning && stripList.SelectedIndices.Count >= 1;
            buttonPreviousEffect.Enabled = _server.IsRunning && stripList.SelectedIndices.Count >= 1;
        }

        // Step back or forward through the effect

        private void buttonPreviousEffect_Click(object sender, EventArgs e)
        {
            var strip = stripList.SelectedItems[0].Tag as LightStrip;
            strip.StripSite.PreviousEffect();
        }

        private void buttonNextEffect_Click(object sender, EventArgs e)
        {
            var strip = stripList.SelectedItems[0].Tag as LightStrip;
            strip.StripSite.NextEffect();

        }

        // Delete a strip from the list

        private void buttonDeleteStrip_Click(object sender, EventArgs e)
        {
            var strip = stripList.SelectedItems[0].Tag as LightStrip;
            _server.RemoveStrip(strip);
            FillListView();
        }

        // Save the strips to a file

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.SaveStrips();
        }

        // Load the strips from a file

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.LoadStrips();
            FillListView();
        }

        // Load pre-baked demo strips from an internal table in lieu of loading from a file or creating all new

        private void loadDemoFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.LoadStripsFromTable();
            FillListView();
        }

        // Souvle-click on a strip to edit it

        private void stripList_DoubleClick(object sender, EventArgs e)
        {
            var strip = stripList.SelectedItems[0].Tag as LightStrip;
            StripDetails details = new StripDetails(_server, strip);
            if (details.ShowDialog() == DialogResult.OK)
            {
                details.StripDetails_Save();
                FillListView();
            }
        }


        private void buttonStartMonitor_Click(object sender, EventArgs e)
        {
            if (!monitorWorker.IsBusy)
            {
                buttonStartMonitor.Enabled = false;
                monitorWorker.RunWorkerAsync();
            }
        }

        private void buttonStopMonitor_Click(object sender, EventArgs e)
        {
            if (monitorWorker.WorkerSupportsCancellation)
                monitorWorker.CancelAsync();
        }

        // StreamReadExact
        //
        // Reads exactly the specified number of bytes from the stream, and waits for it until it gets it

        public byte[] StreamReadExact(NetworkStream stream, uint size)
        {
            byte[] buffer = new byte[size];
            int bytesRead = 0;
            int totalBytesRead = 0;

            while (totalBytesRead < size)
            {
                bytesRead = stream.Read(buffer, totalBytesRead, (int)size - totalBytesRead);
                if (bytesRead == 0)
                {
                    // Connection closed or end of stream reached
                    break;
                }
                totalBytesRead += bytesRead;
            }

            if (totalBytesRead < size)
            {
                throw new IOException("Did not receive the expected number of bytes.");
            }

            return buffer;
        }

        // monitorWorker_DoWork
        //
        // Reads color data from the server and updates the visualizer with that data

        const UInt32 ColorDataPacketHeader = 0x434C5244;
        private void monitorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (; ; )
            {
                try
                {
                    var worker = sender as BackgroundWorker;
                    using (var client = new TcpClient(textColorDataHost.Text, 12000))
                    {
                        using (var stream = client.GetStream())
                        {
                            while (!worker.CancellationPending)
                            {
                                byte[] headerbuffer = StreamReadExact(stream, 3 * 4);

                                UInt32 header = LEDInterop.BytesToDWORD(headerbuffer, 0);
                                UInt32 width = LEDInterop.BytesToDWORD(headerbuffer, 4);
                                UInt32 height = LEDInterop.BytesToDWORD(headerbuffer, 8);

                                if (header != ColorDataPacketHeader)
                                {
                                    Debug.WriteLine("Invalid header received in monitorWorker_DoWork");
                                    worker.CancelAsync();
                                    break;
                                }

                                byte[] bytes = StreamReadExact(stream, width * height * 3);
                                visualizerColorData.fixedWidth = width;
                                visualizerColorData.ColorData = LEDInterop.GetColorsFromBytes(bytes, width * height);
                                visualizerColorData.fixedWidth = width > 1 ? width : 0;
                            }
                            return;
                        }
                    }
                }
                catch (SocketException ex)
                {
                    Debug.WriteLine("SocketException in monitorWorker_DoWork: " + ex.Message);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine("IOException in monitorWorker_DoWork: " + ex.Message);
                }
                Thread.Sleep(1000);
            }
        }

        private void monitorWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was cancelled.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("An error occurred: " + e.Error.Message);
            }
            UpdateUIStates();
        }

        private void NextEffectButton_Click(object sender, EventArgs e)
        {
            foreach (Site site in _server.AllSites.Values)
                site.NextEffect();
        }

        private void PreviousEffectButton_Click(object sender, EventArgs e)
        {
            foreach (Site site in _server.AllSites.Values)
                site.PreviousEffect();

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void stripList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            var group = stripList.SelectedItems[0].Group;
            var site = _server.AllSites[group.Header];
            site.Enabled = e.Item.Checked;
        }
    }
}
