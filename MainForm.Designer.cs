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
            tabControl = new TabControl();
            tabMain = new TabPage();
            splitContainer1 = new SplitContainer();
            buttonDeleteStrip = new Button();
            buttonEditStrip = new Button();
            buttonNewStrip = new Button();
            buttonNextEffect = new Button();
            buttonPreviousEffect = new Button();
            checkGroupItems = new CheckBox();
            panelVisualizer = new LEDVisualizer();
            tabLogging = new TabPage();
            textLog = new TextBox();
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
            statusStrip1.SuspendLayout();
            tabControl.SuspendLayout();
            tabMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabLogging.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // StartButton
            // 
            StartButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            StartButton.Location = new Point(1101, 639);
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
            StopButton.Location = new Point(1020, 639);
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
            statusStrip1.Location = new Point(0, 739);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1209, 22);
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
            stripList.ShowGroups = false;
            stripList.Size = new Size(1178, 290);
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
            // tabControl
            // 
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl.Controls.Add(tabMain);
            tabControl.Controls.Add(tabLogging);
            tabControl.Location = new Point(8, 34);
            tabControl.Margin = new Padding(0);
            tabControl.Name = "tabControl";
            tabControl.Padding = new Point(0, 0);
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1192, 696);
            tabControl.TabIndex = 3;
            // 
            // tabMain
            // 
            tabMain.BackColor = Color.Transparent;
            tabMain.Controls.Add(splitContainer1);
            tabMain.Controls.Add(StopButton);
            tabMain.Controls.Add(StartButton);
            tabMain.ForeColor = Color.Black;
            tabMain.Location = new Point(4, 24);
            tabMain.Name = "tabMain";
            tabMain.Padding = new Padding(3);
            tabMain.Size = new Size(1184, 668);
            tabMain.TabIndex = 0;
            tabMain.Text = "WiFi Control";
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(buttonDeleteStrip);
            splitContainer1.Panel1.Controls.Add(buttonEditStrip);
            splitContainer1.Panel1.Controls.Add(buttonNewStrip);
            splitContainer1.Panel1.Controls.Add(buttonNextEffect);
            splitContainer1.Panel1.Controls.Add(buttonPreviousEffect);
            splitContainer1.Panel1.Controls.Add(checkGroupItems);
            splitContainer1.Panel1.Controls.Add(stripList);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panelVisualizer);
            splitContainer1.Size = new Size(1178, 630);
            splitContainer1.SplitterDistance = 317;
            splitContainer1.TabIndex = 4;
            // 
            // buttonDeleteStrip
            // 
            buttonDeleteStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonDeleteStrip.Location = new Point(938, 293);
            buttonDeleteStrip.Name = "buttonDeleteStrip";
            buttonDeleteStrip.Size = new Size(75, 23);
            buttonDeleteStrip.TabIndex = 4;
            buttonDeleteStrip.Text = "&Delete";
            buttonDeleteStrip.UseVisualStyleBackColor = true;
            buttonDeleteStrip.Click += buttonDeleteStrip_Click;
            // 
            // buttonEditStrip
            // 
            buttonEditStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonEditStrip.Location = new Point(1019, 293);
            buttonEditStrip.Name = "buttonEditStrip";
            buttonEditStrip.Size = new Size(75, 23);
            buttonEditStrip.TabIndex = 4;
            buttonEditStrip.Text = "&Edit...";
            buttonEditStrip.UseVisualStyleBackColor = true;
            // 
            // buttonNewStrip
            // 
            buttonNewStrip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonNewStrip.Location = new Point(1100, 293);
            buttonNewStrip.Name = "buttonNewStrip";
            buttonNewStrip.Size = new Size(75, 23);
            buttonNewStrip.TabIndex = 4;
            buttonNewStrip.Text = "&New...";
            buttonNewStrip.UseVisualStyleBackColor = true;
            // 
            // buttonNextEffect
            // 
            buttonNextEffect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonNextEffect.Location = new Point(243, 292);
            buttonNextEffect.Name = "buttonNextEffect";
            buttonNextEffect.Size = new Size(75, 23);
            buttonNextEffect.TabIndex = 4;
            buttonNextEffect.Text = "Effect &>";
            buttonNextEffect.UseVisualStyleBackColor = true;
            buttonNextEffect.Click += buttonNextEffect_Click;
            // 
            // buttonPreviousEffect
            // 
            buttonPreviousEffect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonPreviousEffect.Location = new Point(162, 292);
            buttonPreviousEffect.Name = "buttonPreviousEffect";
            buttonPreviousEffect.Size = new Size(75, 23);
            buttonPreviousEffect.TabIndex = 4;
            buttonPreviousEffect.Text = "&< Effect";
            buttonPreviousEffect.UseVisualStyleBackColor = true;
            buttonPreviousEffect.Click += buttonPreviousEffect_Click;
            // 
            // checkGroupItems
            // 
            checkGroupItems.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            checkGroupItems.AutoSize = true;
            checkGroupItems.Location = new Point(0, 296);
            checkGroupItems.Name = "checkGroupItems";
            checkGroupItems.Size = new Size(156, 19);
            checkGroupItems.TabIndex = 3;
            checkGroupItems.Text = "Group Items by Location";
            checkGroupItems.UseVisualStyleBackColor = true;
            checkGroupItems.CheckedChanged += checkGroupItems_CheckedChanged;
            // 
            // panelVisualizer
            // 
            panelVisualizer.ColorData = null;
            panelVisualizer.Dock = DockStyle.Fill;
            panelVisualizer.Location = new Point(0, 0);
            panelVisualizer.Name = "panelVisualizer";
            panelVisualizer.Size = new Size(1178, 309);
            panelVisualizer.TabIndex = 3;
            // 
            // tabLogging
            // 
            tabLogging.Controls.Add(textLog);
            tabLogging.Location = new Point(4, 24);
            tabLogging.Name = "tabLogging";
            tabLogging.Padding = new Padding(3);
            tabLogging.Size = new Size(1184, 668);
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
            textLog.Size = new Size(991, 466);
            textLog.TabIndex = 0;
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
            menuStrip1.Size = new Size(1209, 24);
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
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gray;
            ClientSize = new Size(1209, 761);
            Controls.Add(tabControl);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "NightDriver Desktop";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            tabControl.ResumeLayout(false);
            tabMain.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabLogging.ResumeLayout(false);
            tabLogging.PerformLayout();
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
        private TabControl tabControl;
        private TabPage tabMain;
        private TabPage tabLogging;
        private TextBox textLog;
        private LEDVisualizer panelVisualizer;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timerVisualizer;
        private SplitContainer splitContainer1;
        private CheckBox checkGroupItems;
        private Button buttonPreviousEffect;
        private Button buttonDeleteStrip;
        private Button buttonEditStrip;
        private Button buttonNewStrip;
        private Button buttonNextEffect;
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
    }
}
