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


using NightDriverDesktop.Server;
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

        private void StartButton_Click(object sender, EventArgs e)
        {
            NightDriver.ConsoleApp.Stats.WriteLine("Start Server");
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

        private void StopButton_Click(object sender, EventArgs e)
        {
            NightDriver.ConsoleApp.Stats.WriteLine("Stop Server");
            _server.Stop(cancellationTokenSource);
            StartButton.Enabled = true;
            StopButton.Enabled = false;
        }

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
            textLog.Text = NightDriver.ConsoleApp.Stats.Text;
        }

        internal void FillListView()
        {
            stripList.Groups.Clear();
            stripList.Items.Clear();

            foreach (var strip in _server.AllStrips)
            {
                var name = strip.StripSite.Name;
                ListViewGroup group = stripList.Groups[name];
                if (group == null)
                    group = stripList.Groups.Add(name, name);
                stripList.Items.Add(new StripListItem(group, strip.FriendlyName, strip));
            }
            if (stripList.Items.Count > 0)
                stripList.Items[0].Selected = true;
            UpdateUIStates();
        }

        internal void UpdateListView()
        {
            foreach (var strip in _server.AllStrips)
            {
                var item = stripList.FindItemWithText(strip.FriendlyName, true, 0) as StripListItem;
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

        private void stripList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stripList.SelectedIndices.Count > 0)
            {
                var strip = stripList.SelectedItems[0].Tag as LightStrip;
                panelVisualizer.ColorData = strip.StripSite.LEDs;
                timerVisualizer.Interval = Math.Clamp(1000 / strip.StripSite.FramesPerSecond, 20, 500);
            }
            UpdateUIStates();
        }

        private void timerVisualizer_Tick(object sender, EventArgs e)
        {
            panelVisualizer.Invalidate();
        }

        private void checkGroupItems_CheckedChanged(object sender, EventArgs e)
        {
            stripList.ShowGroups = checkGroupItems.Checked;
            FillListView();
        }

        private void stripList_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var comparer = stripList.ListViewItemSorter as StripListItemComparer;
            if (comparer != null && comparer.Column == (StripListViewColumnIndex)e.Column)
                comparer.ToggleSortOrder();
            else
                stripList.ListViewItemSorter = new StripListItemComparer((StripListViewColumnIndex)e.Column);

            stripList.Sort();
        }

        private void UpdateUIStates()
        {
            checkGroupItems.Checked = stripList.ShowGroups;
            checkGroupItems.Enabled = stripList.Groups.Count > 0;

            buttonDeleteStrip.Enabled = !_server.IsRunning && stripList.SelectedIndices.Count > 0;
            buttonEditStrip.Enabled = !_server.IsRunning && stripList.SelectedIndices.Count == 1;
            buttonNewStrip.Enabled = !_server.IsRunning;

            buttonStartMonitor.Enabled = !monitorWorker.IsBusy;
            buttonStopMonitor.Enabled = monitorWorker.IsBusy;
        }

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

        private void buttonDeleteStrip_Click(object sender, EventArgs e)
        {
            var strip = stripList.SelectedItems[0].Tag as LightStrip;
            _server.RemoveStrip(strip);
            FillListView();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.SaveStrips();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.LoadStrips();
            FillListView();
        }

        private void loadDemoFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _server.LoadStripsFromTable();
            FillListView();
        }

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
                bytesRead = stream.Read(buffer, totalBytesRead, (int) size - totalBytesRead);
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
            for (;;)
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
                                UInt32 width  = LEDInterop.BytesToDWORD(headerbuffer, 4);
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
    }
}
