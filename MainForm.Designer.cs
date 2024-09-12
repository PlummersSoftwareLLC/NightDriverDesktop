
namespace NightDriver
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            StartButton = new Button();
            StopButton = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            timerListView = new System.Windows.Forms.Timer(components);
            stripList = new StripListView();
            colName = new ColumnHeader();
            colHost = new ColumnHeader();
            colSocket = new ColumnHeader();
            colWiFi = new ColumnHeader();
            colStatus = new ColumnHeader();
            colBPS = new ColumnHeader();
            colClock = new ColumnHeader();
            colBuffer = new ColumnHeader();
            colPower = new ColumnHeader();
            colFPS = new ColumnHeader();
            colOffset = new ColumnHeader();
            colConnects = new ColumnHeader();
            colQueue = new ColumnHeader();
            colEffect = new ColumnHeader();
            tabColorData = new TabControl();
            tabMain = new TabPage();
            splitContainer1 = new SplitContainer();
            groupBox2 = new GroupBox();
            buttonDeleteStrip = new Button();
            buttonEditStrip = new Button();
            buttonNewStrip = new Button();
            groupBox1 = new GroupBox();
            buttonDeleteLocation = new Button();
            buttonEditLocation = new Button();
            buttonNewLocation = new Button();
            panelVisualizer = new LEDVisualizer();
            buttonNextEffect = new Button();
            buttonPreviousEffect = new Button();
            tabLocations = new TabPage();
            splitContainer2 = new SplitContainer();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            listLocations = new ListView();
            columnLocartion = new ColumnHeader();
            columnWidth = new ColumnHeader();
            columnHeight = new ColumnHeader();
            columnFPS = new ColumnHeader();
            columnEffect = new ColumnHeader();
            ledVisualizer1 = new LEDVisualizer();
            tabLogging = new TabPage();
            textLog = new TextBox();
            tabColor = new TabPage();
            buttonStopMonitor = new Button();
            label1 = new Label();
            buttonStartMonitor = new Button();
            textColorDataHost = new TextBox();
            visualizerColorData = new LEDVisualizer();
            timer1 = new System.Windows.Forms.Timer(components);
            timerVisualizer = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            menuFile = new ToolStripMenuItem();
            loadDemoFileToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            newToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            newEntryToolStripMenuItem = new ToolStripMenuItem();
            editEntryToolStripMenuItem = new ToolStripMenuItem();
            deleteEntryToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            updateSpeedToolStripMenuItem = new ToolStripMenuItem();
            pausedToolStripMenuItem = new ToolStripMenuItem();
            lowToolStripMenuItem = new ToolStripMenuItem();
            mediumToolStripMenuItem = new ToolStripMenuItem();
            highToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            refreshToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            refreshToolStripMenuItem1 = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            monitorWorker = new System.ComponentModel.BackgroundWorker();
            statusStrip1.SuspendLayout();
            tabColorData.SuspendLayout();
            tabMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabLocations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabLogging.SuspendLayout();
            tabColor.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // StartButton
            // 
            StartButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            StartButton.Location = new Point(1091, 256);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(75, 23);
            StartButton.TabIndex = 0;
            StartButton.Text = "Start";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // StopButton
            // 
            StopButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            StopButton.Enabled = false;
            StopButton.Location = new Point(1010, 256);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(75, 23);
            StopButton.TabIndex = 0;
            StopButton.Text = "Stop";
            StopButton.UseVisualStyleBackColor = true;
            StopButton.Click += StopButton_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 648);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1200, 22);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BackColor = SystemColors.ButtonFace;
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // timerListView
            // 
            timerListView.Interval = 50;
            timerListView.Tick += timer1_Tick;
            // 
            // stripList
            // 
            stripList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            stripList.BackColor = Color.Gainsboro;
            stripList.Columns.AddRange(new ColumnHeader[] { colName, colHost, colSocket, colWiFi, colStatus, colBPS, colClock, colBuffer, colPower, colFPS, colOffset, colConnects, colQueue, colEffect });
            stripList.ForeColor = Color.Black;
            stripList.FullRowSelect = true;
            stripList.Location = new Point(0, 0);
            stripList.Name = "stripList";
            stripList.OwnerDraw = true;
            stripList.ShowGroups = false;
            stripList.Size = new Size(1169, 209);
            stripList.Sorting = SortOrder.Ascending;
            stripList.TabIndex = 2;
            stripList.UseCompatibleStateImageBehavior = false;
            stripList.View = View.Details;
            stripList.ColumnClick += stripList_ColumnClick;
            stripList.SelectedIndexChanged += stripList_SelectedIndexChanged;
            stripList.DoubleClick += stripList_DoubleClick;
            // 
            // colName
            // 
            colName.Text = "Name";
            colName.Width = 100;
            // 
            // colHost
            // 
            colHost.Text = "Host";
            colHost.Width = 100;
            // 
            // colSocket
            // 
            colSocket.Text = "Socket";
            colSocket.Width = 50;
            // 
            // colWiFi
            // 
            colWiFi.Text = "WiFi";
            colWiFi.Width = 40;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 50;
            // 
            // colBPS
            // 
            colBPS.Text = "Bytes/Sec";
            colBPS.Width = 80;
            // 
            // colClock
            // 
            colClock.Text = "Clock";
            colClock.Width = 50;
            // 
            // colBuffer
            // 
            colBuffer.Text = "Buffer";
            colBuffer.Width = 70;
            // 
            // colPower
            // 
            colPower.Text = "Power";
            colPower.Width = 50;
            // 
            // colFPS
            // 
            colFPS.Text = "FPS";
            colFPS.Width = 40;
            // 
            // colOffset
            // 
            colOffset.DisplayIndex = 12;
            colOffset.Text = "Offset";
            colOffset.Width = 50;
            // 
            // colConnects
            // 
            colConnects.DisplayIndex = 10;
            colConnects.Text = "Connects";
            colConnects.Width = 70;
            // 
            // colQueue
            // 
            colQueue.DisplayIndex = 11;
            colQueue.Text = "Queue";
            colQueue.Width = 50;
            // 
            // colEffect
            // 
            colEffect.Text = "Effect";
            colEffect.Width = 190;
            // 
            // tabColorData
            // 
            tabColorData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabColorData.Controls.Add(tabMain);
            tabColorData.Controls.Add(tabLocations);
            tabColorData.Controls.Add(tabLogging);
            tabColorData.Controls.Add(tabColor);
            tabColorData.Location = new Point(8, 34);
            tabColorData.Margin = new Padding(0);
            tabColorData.Name = "tabColorData";
            tabColorData.Padding = new Point(0, 0);
            tabColorData.SelectedIndex = 0;
            tabColorData.Size = new Size(1183, 607);
            tabColorData.TabIndex = 3;
            // 
            // tabMain
            // 
            tabMain.BackColor = Color.Transparent;
            tabMain.Controls.Add(splitContainer1);
            tabMain.ForeColor = Color.Black;
            tabMain.Location = new Point(4, 24);
            tabMain.Name = "tabMain";
            tabMain.Padding = new Padding(3);
            tabMain.Size = new Size(1175, 579);
            tabMain.TabIndex = 0;
            tabMain.Text = "WiFi Control";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox2);
            splitContainer1.Panel1.Controls.Add(groupBox1);
            splitContainer1.Panel1.Controls.Add(stripList);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panelVisualizer);
            splitContainer1.Panel2.Controls.Add(buttonNextEffect);
            splitContainer1.Panel2.Controls.Add(buttonPreviousEffect);
            splitContainer1.Panel2.Controls.Add(StopButton);
            splitContainer1.Panel2.Controls.Add(StartButton);
            splitContainer1.Size = new Size(1169, 573);
            splitContainer1.SplitterDistance = 287;
            splitContainer1.TabIndex = 4;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox2.Controls.Add(buttonDeleteStrip);
            groupBox2.Controls.Add(buttonEditStrip);
            groupBox2.Controls.Add(buttonNewStrip);
            groupBox2.Location = new Point(907, 215);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(259, 55);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Devices";
            // 
            // buttonDeleteStrip
            // 
            buttonDeleteStrip.Location = new Point(9, 22);
            buttonDeleteStrip.Name = "buttonDeleteStrip";
            buttonDeleteStrip.Size = new Size(75, 23);
            buttonDeleteStrip.TabIndex = 5;
            buttonDeleteStrip.Text = "&Delete";
            buttonDeleteStrip.UseVisualStyleBackColor = true;
            // 
            // buttonEditStrip
            // 
            buttonEditStrip.Location = new Point(90, 22);
            buttonEditStrip.Name = "buttonEditStrip";
            buttonEditStrip.Size = new Size(75, 23);
            buttonEditStrip.TabIndex = 6;
            buttonEditStrip.Text = "&Edit...";
            buttonEditStrip.UseVisualStyleBackColor = true;
            // 
            // buttonNewStrip
            // 
            buttonNewStrip.Location = new Point(171, 22);
            buttonNewStrip.Name = "buttonNewStrip";
            buttonNewStrip.Size = new Size(75, 23);
            buttonNewStrip.TabIndex = 7;
            buttonNewStrip.Text = "&New...";
            buttonNewStrip.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox1.Controls.Add(buttonDeleteLocation);
            groupBox1.Controls.Add(buttonEditLocation);
            groupBox1.Controls.Add(buttonNewLocation);
            groupBox1.Location = new Point(645, 215);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(256, 55);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Locations";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // buttonDeleteLocation
            // 
            buttonDeleteLocation.Location = new Point(7, 22);
            buttonDeleteLocation.Name = "buttonDeleteLocation";
            buttonDeleteLocation.Size = new Size(75, 23);
            buttonDeleteLocation.TabIndex = 5;
            buttonDeleteLocation.Text = "&Delete";
            buttonDeleteLocation.UseVisualStyleBackColor = true;
            // 
            // buttonEditLocation
            // 
            buttonEditLocation.Location = new Point(88, 22);
            buttonEditLocation.Name = "buttonEditLocation";
            buttonEditLocation.Size = new Size(75, 23);
            buttonEditLocation.TabIndex = 6;
            buttonEditLocation.Text = "&Edit...";
            buttonEditLocation.UseVisualStyleBackColor = true;
            // 
            // buttonNewLocation
            // 
            buttonNewLocation.Location = new Point(169, 22);
            buttonNewLocation.Name = "buttonNewLocation";
            buttonNewLocation.Size = new Size(75, 23);
            buttonNewLocation.TabIndex = 7;
            buttonNewLocation.Text = "&New...";
            buttonNewLocation.UseVisualStyleBackColor = true;
            // 
            // panelVisualizer
            // 
            panelVisualizer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelVisualizer.ColorData = null;
            panelVisualizer.Location = new Point(0, 0);
            panelVisualizer.Name = "panelVisualizer";
            panelVisualizer.Size = new Size(1166, 250);
            panelVisualizer.TabIndex = 3;
            // 
            // buttonNextEffect
            // 
            buttonNextEffect.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonNextEffect.Enabled = false;
            buttonNextEffect.Location = new Point(81, 256);
            buttonNextEffect.Name = "buttonNextEffect";
            buttonNextEffect.Size = new Size(75, 23);
            buttonNextEffect.TabIndex = 0;
            buttonNextEffect.Text = "Next >";
            buttonNextEffect.UseVisualStyleBackColor = true;
            buttonNextEffect.Click += NextEffectButton_Click;
            // 
            // buttonPreviousEffect
            // 
            buttonPreviousEffect.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonPreviousEffect.Enabled = false;
            buttonPreviousEffect.Location = new Point(0, 256);
            buttonPreviousEffect.Name = "buttonPreviousEffect";
            buttonPreviousEffect.Size = new Size(75, 23);
            buttonPreviousEffect.TabIndex = 0;
            buttonPreviousEffect.Text = "< Previous";
            buttonPreviousEffect.UseVisualStyleBackColor = true;
            buttonPreviousEffect.Click += PreviousEffectButton_Click;
            // 
            // tabLocations
            // 
            tabLocations.Controls.Add(splitContainer2);
            tabLocations.Location = new Point(4, 24);
            tabLocations.Name = "tabLocations";
            tabLocations.Padding = new Padding(3);
            tabLocations.Size = new Size(1175, 579);
            tabLocations.TabIndex = 2;
            tabLocations.Text = "Locations";
            tabLocations.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            splitContainer2.BorderStyle = BorderStyle.Fixed3D;
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(3, 3);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(button1);
            splitContainer2.Panel1.Controls.Add(button2);
            splitContainer2.Panel1.Controls.Add(button3);
            splitContainer2.Panel1.Controls.Add(listLocations);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(ledVisualizer1);
            splitContainer2.Size = new Size(1169, 573);
            splitContainer2.SplitterDistance = 336;
            splitContainer2.TabIndex = 0;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(925, 306);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 5;
            button1.Text = "&Delete";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.Location = new Point(1006, 306);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 6;
            button2.Text = "&Edit...";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.Location = new Point(1087, 306);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 7;
            button3.Text = "&New...";
            button3.UseVisualStyleBackColor = true;
            // 
            // listLocations
            // 
            listLocations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listLocations.Columns.AddRange(new ColumnHeader[] { columnLocartion, columnWidth, columnHeight, columnFPS, columnEffect });
            listLocations.Location = new Point(3, 3);
            listLocations.Name = "listLocations";
            listLocations.Size = new Size(1159, 297);
            listLocations.TabIndex = 0;
            listLocations.UseCompatibleStateImageBehavior = false;
            listLocations.View = View.Details;
            // 
            // columnLocartion
            // 
            columnLocartion.Text = "Location Name";
            columnLocartion.Width = 200;
            // 
            // columnWidth
            // 
            columnWidth.Text = "Width";
            // 
            // columnHeight
            // 
            columnHeight.Text = "Height";
            // 
            // columnFPS
            // 
            columnFPS.Text = "Frames Per Second";
            columnFPS.Width = 150;
            // 
            // columnEffect
            // 
            columnEffect.Text = "Current Effect";
            columnEffect.Width = 200;
            // 
            // ledVisualizer1
            // 
            ledVisualizer1.ColorData = null;
            ledVisualizer1.Dock = DockStyle.Fill;
            ledVisualizer1.Location = new Point(0, 0);
            ledVisualizer1.Name = "ledVisualizer1";
            ledVisualizer1.Size = new Size(1165, 229);
            ledVisualizer1.TabIndex = 4;
            // 
            // tabLogging
            // 
            tabLogging.Controls.Add(textLog);
            tabLogging.Location = new Point(4, 24);
            tabLogging.Name = "tabLogging";
            tabLogging.Padding = new Padding(3);
            tabLogging.Size = new Size(1175, 579);
            tabLogging.TabIndex = 1;
            tabLogging.Text = "Logging";
            tabLogging.UseVisualStyleBackColor = true;
            // 
            // textLog
            // 
            textLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textLog.BackColor = Color.Black;
            textLog.ForeColor = Color.Lime;
            textLog.Location = new Point(0, 0);
            textLog.Multiline = true;
            textLog.Name = "textLog";
            textLog.ScrollBars = ScrollBars.Vertical;
            textLog.Size = new Size(1205, 801);
            textLog.TabIndex = 0;
            // 
            // tabColor
            // 
            tabColor.Controls.Add(buttonStopMonitor);
            tabColor.Controls.Add(label1);
            tabColor.Controls.Add(buttonStartMonitor);
            tabColor.Controls.Add(textColorDataHost);
            tabColor.Controls.Add(visualizerColorData);
            tabColor.Location = new Point(4, 24);
            tabColor.Name = "tabColor";
            tabColor.Padding = new Padding(3);
            tabColor.Size = new Size(1175, 579);
            tabColor.TabIndex = 3;
            tabColor.Text = "Color Data";
            tabColor.UseVisualStyleBackColor = true;
            // 
            // buttonStopMonitor
            // 
            buttonStopMonitor.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonStopMonitor.Enabled = false;
            buttonStopMonitor.Location = new Point(1013, 550);
            buttonStopMonitor.Name = "buttonStopMonitor";
            buttonStopMonitor.Size = new Size(75, 23);
            buttonStopMonitor.TabIndex = 1;
            buttonStopMonitor.Text = "Stop";
            buttonStopMonitor.UseVisualStyleBackColor = true;
            buttonStopMonitor.Click += buttonStopMonitor_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 13);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 7;
            label1.Text = "IP or Hostname";
            // 
            // buttonStartMonitor
            // 
            buttonStartMonitor.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonStartMonitor.Location = new Point(1094, 550);
            buttonStartMonitor.Name = "buttonStartMonitor";
            buttonStartMonitor.Size = new Size(75, 23);
            buttonStartMonitor.TabIndex = 2;
            buttonStartMonitor.Text = "Start";
            buttonStartMonitor.UseVisualStyleBackColor = true;
            buttonStartMonitor.Click += buttonStartMonitor_Click;
            // 
            // textColorDataHost
            // 
            textColorDataHost.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textColorDataHost.Location = new Point(107, 10);
            textColorDataHost.Name = "textColorDataHost";
            textColorDataHost.Size = new Size(424, 23);
            textColorDataHost.TabIndex = 6;
            textColorDataHost.Text = "192.168.8.235";
            // 
            // visualizerColorData
            // 
            visualizerColorData.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            visualizerColorData.ColorData = null;
            visualizerColorData.Location = new Point(3, 39);
            visualizerColorData.Name = "visualizerColorData";
            visualizerColorData.Size = new Size(1176, 505);
            visualizerColorData.TabIndex = 5;
            // 
            // timerVisualizer
            // 
            timerVisualizer.Tick += timerVisualizer_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { menuFile, editToolStripMenuItem, viewToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1200, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new ToolStripItem[] { loadDemoFileToolStripMenuItem, toolStripSeparator4, newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(37, 20);
            menuFile.Text = "&File";
            // 
            // loadDemoFileToolStripMenuItem
            // 
            loadDemoFileToolStripMenuItem.Name = "loadDemoFileToolStripMenuItem";
            loadDemoFileToolStripMenuItem.Size = new Size(156, 22);
            loadDemoFileToolStripMenuItem.Text = "Load Demo File";
            loadDemoFileToolStripMenuItem.Click += loadDemoFileToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(153, 6);
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(156, 22);
            newToolStripMenuItem.Text = "&New...";
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(156, 22);
            openToolStripMenuItem.Text = "Open...";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(156, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(156, 22);
            saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(153, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(156, 22);
            exitToolStripMenuItem.Text = "E&xit";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newEntryToolStripMenuItem, editEntryToolStripMenuItem, deleteEntryToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "&Edit";
            // 
            // newEntryToolStripMenuItem
            // 
            newEntryToolStripMenuItem.Name = "newEntryToolStripMenuItem";
            newEntryToolStripMenuItem.Size = new Size(137, 22);
            newEntryToolStripMenuItem.Text = "New Entry...";
            // 
            // editEntryToolStripMenuItem
            // 
            editEntryToolStripMenuItem.Name = "editEntryToolStripMenuItem";
            editEntryToolStripMenuItem.Size = new Size(137, 22);
            editEntryToolStripMenuItem.Text = "Edit Entry...";
            // 
            // deleteEntryToolStripMenuItem
            // 
            deleteEntryToolStripMenuItem.Name = "deleteEntryToolStripMenuItem";
            deleteEntryToolStripMenuItem.Size = new Size(137, 22);
            deleteEntryToolStripMenuItem.Text = "Delete Entry";
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { updateSpeedToolStripMenuItem, toolStripSeparator3, refreshToolStripMenuItem1 });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // updateSpeedToolStripMenuItem
            // 
            updateSpeedToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { pausedToolStripMenuItem, lowToolStripMenuItem, mediumToolStripMenuItem, highToolStripMenuItem, toolStripSeparator2, refreshToolStripMenuItem });
            updateSpeedToolStripMenuItem.Name = "updateSpeedToolStripMenuItem";
            updateSpeedToolStripMenuItem.Size = new Size(147, 22);
            updateSpeedToolStripMenuItem.Text = "Update Speed";
            // 
            // pausedToolStripMenuItem
            // 
            pausedToolStripMenuItem.Name = "pausedToolStripMenuItem";
            pausedToolStripMenuItem.Size = new Size(119, 22);
            pausedToolStripMenuItem.Text = "&Paused";
            // 
            // lowToolStripMenuItem
            // 
            lowToolStripMenuItem.Name = "lowToolStripMenuItem";
            lowToolStripMenuItem.Size = new Size(119, 22);
            lowToolStripMenuItem.Text = "&Low";
            // 
            // mediumToolStripMenuItem
            // 
            mediumToolStripMenuItem.Name = "mediumToolStripMenuItem";
            mediumToolStripMenuItem.Size = new Size(119, 22);
            mediumToolStripMenuItem.Text = "&Medium";
            // 
            // highToolStripMenuItem
            // 
            highToolStripMenuItem.Name = "highToolStripMenuItem";
            highToolStripMenuItem.Size = new Size(119, 22);
            highToolStripMenuItem.Text = "&High";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(116, 6);
            // 
            // refreshToolStripMenuItem
            // 
            refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            refreshToolStripMenuItem.Size = new Size(119, 22);
            refreshToolStripMenuItem.Text = "Refresh";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(144, 6);
            // 
            // refreshToolStripMenuItem1
            // 
            refreshToolStripMenuItem1.Name = "refreshToolStripMenuItem1";
            refreshToolStripMenuItem1.Size = new Size(147, 22);
            refreshToolStripMenuItem1.Text = "Refresh";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(116, 22);
            aboutToolStripMenuItem.Text = "About...";
            // 
            // monitorWorker
            // 
            monitorWorker.WorkerSupportsCancellation = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1200, 670);
            Controls.Add(tabColorData);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "F";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabColorData.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            tabLocations.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabLogging.ResumeLayout(false);
            tabLogging.PerformLayout();
            tabColor.ResumeLayout(false);
            tabColor.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button StartButton;
        private Button StopButton;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer timerListView;
        private StripListView stripList;
        private ColumnHeader colName;
        private ColumnHeader colHost;
        private ColumnHeader colSocket;
        private ColumnHeader colWiFi;
        private ColumnHeader colStatus;
        private ColumnHeader colBPS;
        private ColumnHeader colClock;
        private ColumnHeader colBuffer;
        private ColumnHeader colPower;
        private ColumnHeader colFPS;
        private ColumnHeader colConnects;
        private ColumnHeader colQueue;
        private ColumnHeader colOffset;
        private ColumnHeader colEffect;
        private TabControl tabColorData;
        private TabPage tabMain;
        private TabPage tabLogging;
        private TextBox textLog;
        private LEDVisualizer panelVisualizer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerVisualizer;
        private SplitContainer splitContainer1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem newEntryToolStripMenuItem;
        private ToolStripMenuItem editEntryToolStripMenuItem;
        private ToolStripMenuItem deleteEntryToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem updateSpeedToolStripMenuItem;
        private ToolStripMenuItem pausedToolStripMenuItem;
        private ToolStripMenuItem lowToolStripMenuItem;
        private ToolStripMenuItem mediumToolStripMenuItem;
        private ToolStripMenuItem highToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem refreshToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem refreshToolStripMenuItem1;
        private ToolStripMenuItem loadDemoFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private TabPage tabLocations;
        private SplitContainer splitContainer2;
        private LEDVisualizer ledVisualizer1;
        private ListView listLocations;
        private Button button1;
        private Button button2;
        private Button button3;
        private ColumnHeader columnLocartion;
        private ColumnHeader columnWidth;
        private ColumnHeader columnHeight;
        private ColumnHeader columnFPS;
        private ColumnHeader columnEffect;
        private TabPage tabColor;
        private LEDVisualizer visualizerColorData;
        private TextBox textColorDataHost;
        private Label label1;
        private Button buttonStopMonitor;
        private Button buttonStartMonitor;
        private System.ComponentModel.BackgroundWorker monitorWorker;
        private Button buttonNextEffect;
        private Button buttonPreviousEffect;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button buttonDeleteStrip;
        private Button buttonEditStrip;
        private Button buttonNewStrip;
        private Button buttonDeleteLocation;
        private Button buttonEditLocation;
        private Button buttonNewLocation;
    }
}
