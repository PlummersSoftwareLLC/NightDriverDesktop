using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace NightDriver
{
    internal partial class StripDetails : Form
    {
        private LEDServer _server;
        private LightStrip _strip;

        internal StripDetails()
        {
            InitializeComponent();
        }

        internal StripDetails(LEDServer server, LightStrip strip) : this()
        {
            _server = server; 
            _strip = strip;

            textHostName.Text = strip.HostName;
            textName.Text = strip.FriendlyName;
            textWidth.Text = strip.Width.ToString();
            textHeight.Text = strip.Height.ToString();
            textOffset.Text = strip.Offset.ToString();
            textBatchSize.Text = strip.BatchSize.ToString();
            checkCompress.Checked = strip.CompressData;
            checkSwapRedGreen.Checked = strip.RedGreenSwap;
            checkReverse.Checked = strip.Reversed;
            comboChannel.SelectedIndex = strip.Channel;

            foreach (var site in _server.AllSites.Values)
                comboLocation.Items.Add(site.Name);

            if (_strip.StripSite != null)
                comboLocation.SelectedItem = _strip.StripSite.Name; 
        }

        internal bool StripDetails_Save()
        {
            // First we validate the inputs

            // Hostname must start with a letter and contain only letters and numbers

            // Regex pattern for a basic hostname (simplified)
            const string hostnamePattern = @"^([a-zA-Z][a-zA-Z0-9\-]*\.)*[a-zA-Z][a-zA-Z0-9\-]*$";

            // Regex pattern for IPv4 addresses
            const string ipv4Pattern = @"^(\d{1,3}\.){3}\d{1,3}$";

            // Regex pattern for IPv6 addresses (very simplified version)
            const string ipv6Pattern = @"^([0-9a-fA-F]{1,4}:){7}([0-9a-fA-F]{1,4})$";

            // Check if the text matches any of the patterns
            if (!Regex.IsMatch(textHostName.Text, hostnamePattern) && !Regex.IsMatch(textHostName.Text, ipv4Pattern) && !Regex.IsMatch(textHostName.Text, ipv6Pattern))
            {
                MessageBox.Show("Invalid hostname. Please enter a valid hostname or IP address.");
                return false;
            }

            if (!Regex.IsMatch(textName.Text, @"^[A-Za-z][A-Za-z0-9]*$"))
            {
                MessageBox.Show("Invalid name, must start with a letter and contain only letters and numbers.");
                return false;
            }

            if (!textWidth.Text.IsInRange(1, 10000, out int width))
            {
                MessageBox.Show("Width must be between 1 and 10000");
                return false;
            }

            if (!textHeight.Text.IsInRange(1, 10000, out int height))
            {
                MessageBox.Show("Width must be between 1 and 10000");
                return false;
            }

            if (!textOffset.Text.IsInRange(0, 10000, out int offset))
            {
                MessageBox.Show("Offset must be between 0 and 10000");
                return false;
            }

            if (!textBatchSize.Text.IsInRange(1, 1000, out int batchSize))
            {
                MessageBox.Show("Batch size must be between 1 and 1000");
                return false;
            }

            // Everything validated so we can save the values, and should be safe to parse them

            _strip.HostName = textHostName.Text;
            _strip.FriendlyName = textName.Text;
            _strip.Width = uint.Parse(textWidth.Text);
            _strip.Height = uint.Parse(textHeight.Text);
            _strip.Offset = uint.Parse(textOffset.Text);
            _strip.BatchSize = uint.Parse(textBatchSize.Text);
            _strip.CompressData = checkCompress.Checked;
            _strip.RedGreenSwap = checkSwapRedGreen.Checked;
            _strip.Reversed = checkReverse.Checked;
            _strip.Channel = (byte) comboChannel.SelectedIndex;
            return true;
        }

        // User has clicked the Apply button, so we save the values and close the dialog if

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (StripDetails_Save())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
