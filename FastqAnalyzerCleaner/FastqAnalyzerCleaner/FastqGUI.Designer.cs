// <copyright file="FastqGUI.Designer.cs" author="Neil Robertson">
// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
//
// This code is the property of Neil Robertson.  Permission must be sought before reuse.
// It has been written explicitly for the MRes Bioinfomatics course at the University 
// of Glasgow, Scotland under the supervision of Derek Gatherer.
//
// </copyright>
// <author>Neil Robertson</author>
// <email>neil.alistair.robertson@hotmail.co.uk</email>
// <date>2013-06-1</date>

using System;

namespace FastqAnalyzerCleaner
{
    partial class FastqGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        { 
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FastqGUI));
            this.OpenFastqDialogue = new System.Windows.Forms.OpenFileDialog();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFastqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveFastqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCSVDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.flushMemoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rescanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reanalyzeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSequenceStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clean3EndsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clean5EndsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanTailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSequencesWithFailedReadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.withFailedReadsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.belowMeanThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.removeAdapterSequencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.findSequenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.preferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.progressStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Clean_Sweep_Radio = new System.Windows.Forms.RadioButton();
            this.Decision_Tree_Radio = new System.Windows.Forms.RadioButton();
            this.Single_Core_Radio = new System.Windows.Forms.RadioButton();
            this.Multi_Core_Radio = new System.Windows.Forms.RadioButton();
            this.Cores_Group_Box = new System.Windows.Forms.GroupBox();
            this.Sequencer_Selection_Group_Box = new System.Windows.Forms.GroupBox();
            this.FastqGUITabs = new System.Windows.Forms.TabControl();
            this.InformationTab = new System.Windows.Forms.TabPage();
            this.FastqGUI_Display = new System.Windows.Forms.RichTextBox();
            this.GraphicsTab = new System.Windows.Forms.TabPage();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.Charts_Combo_Selector = new System.Windows.Forms.ComboBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.MenuStrip.SuspendLayout();
            this.progressStrip.SuspendLayout();
            this.Cores_Group_Box.SuspendLayout();
            this.Sequencer_Selection_Group_Box.SuspendLayout();
            this.FastqGUITabs.SuspendLayout();
            this.InformationTab.SuspendLayout();
            this.GraphicsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.actionToolStripMenuItem,
            this.editToolStripMenuItem,
            this.preferencesToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(934, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "MenuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFastqToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveFastqToolStripMenuItem,
            this.saveCSVDataToolStripMenuItem,
            this.toolStripSeparator2,
            this.flushMemoryToolStripMenuItem,
            this.toolStripSeparator6,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fileToolStripMenuItem.Text = "FILE";
            // 
            // openFastqToolStripMenuItem
            // 
            this.openFastqToolStripMenuItem.Name = "openFastqToolStripMenuItem";
            this.openFastqToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.openFastqToolStripMenuItem.Text = "Open Fastq...";
            this.openFastqToolStripMenuItem.Click += new System.EventHandler(this.openFastqFile);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(155, 6);
            // 
            // saveFastqToolStripMenuItem
            // 
            this.saveFastqToolStripMenuItem.Name = "saveFastqToolStripMenuItem";
            this.saveFastqToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveFastqToolStripMenuItem.Text = "Save Fastq...";
            this.saveFastqToolStripMenuItem.Click += new System.EventHandler(this.saveFastqToolStripMenuItem_Click);
            // 
            // saveCSVDataToolStripMenuItem
            // 
            this.saveCSVDataToolStripMenuItem.Name = "saveCSVDataToolStripMenuItem";
            this.saveCSVDataToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.saveCSVDataToolStripMenuItem.Text = "Save CSV Data...";
            this.saveCSVDataToolStripMenuItem.Click += new System.EventHandler(this.saveCSVDataToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(155, 6);
            // 
            // flushMemoryToolStripMenuItem
            // 
            this.flushMemoryToolStripMenuItem.Name = "flushMemoryToolStripMenuItem";
            this.flushMemoryToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.flushMemoryToolStripMenuItem.Text = "Flush Memory";
            this.flushMemoryToolStripMenuItem.Click += new System.EventHandler(this.flushMemoryToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(155, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // actionToolStripMenuItem
            // 
            this.actionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanToolStripMenuItem,
            this.reanalyzeToolStripMenuItem,
            this.showSequenceStatisticsToolStripMenuItem});
            this.actionToolStripMenuItem.Name = "actionToolStripMenuItem";
            this.actionToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.actionToolStripMenuItem.Text = "ACTION";
            // 
            // rescanToolStripMenuItem
            // 
            this.rescanToolStripMenuItem.Name = "rescanToolStripMenuItem";
            this.rescanToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.rescanToolStripMenuItem.Text = "Rescan";
            this.rescanToolStripMenuItem.Click += new System.EventHandler(this.rescanToolStripMenuItem_Click);
            // 
            // reanalyzeToolStripMenuItem
            // 
            this.reanalyzeToolStripMenuItem.Name = "reanalyzeToolStripMenuItem";
            this.reanalyzeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.reanalyzeToolStripMenuItem.Text = "Reanalyze";
            this.reanalyzeToolStripMenuItem.Click += new System.EventHandler(this.reanalyzeToolStripMenuItem_Click);
            // 
            // showSequenceStatisticsToolStripMenuItem
            // 
            this.showSequenceStatisticsToolStripMenuItem.Name = "showSequenceStatisticsToolStripMenuItem";
            this.showSequenceStatisticsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.showSequenceStatisticsToolStripMenuItem.Text = "Show Sequence Statistics";
            this.showSequenceStatisticsToolStripMenuItem.Click += new System.EventHandler(this.showSequenceStatisticsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clean3EndsToolStripMenuItem,
            this.clean5EndsToolStripMenuItem,
            this.cleanTailsToolStripMenuItem,
            this.removeSequencesWithFailedReadsToolStripMenuItem,
            this.toolStripSeparator3,
            this.removeAdapterSequencesToolStripMenuItem,
            this.toolStripSeparator4,
            this.findSequenceToolStripMenuItem,
            this.toolStripSeparator5});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.editToolStripMenuItem.Text = "EDIT";
            // 
            // clean3EndsToolStripMenuItem
            // 
            this.clean3EndsToolStripMenuItem.Name = "clean3EndsToolStripMenuItem";
            this.clean3EndsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.clean3EndsToolStripMenuItem.Text = "Trim 3\' Ends";
            this.clean3EndsToolStripMenuItem.Click += new System.EventHandler(this.clean3EndsToolStripMenuItem_Click);
            // 
            // clean5EndsToolStripMenuItem
            // 
            this.clean5EndsToolStripMenuItem.Name = "clean5EndsToolStripMenuItem";
            this.clean5EndsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.clean5EndsToolStripMenuItem.Text = "Trim 5\' Ends";
            this.clean5EndsToolStripMenuItem.Click += new System.EventHandler(this.clean5EndsToolStripMenuItem_Click);
            // 
            // cleanTailsToolStripMenuItem
            // 
            this.cleanTailsToolStripMenuItem.Name = "cleanTailsToolStripMenuItem";
            this.cleanTailsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.cleanTailsToolStripMenuItem.Text = "Trim Tails";
            this.cleanTailsToolStripMenuItem.Click += new System.EventHandler(this.cleanTailsToolStripMenuItem_Click);
            // 
            // removeSequencesWithFailedReadsToolStripMenuItem
            // 
            this.removeSequencesWithFailedReadsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.withFailedReadsToolStripMenuItem,
            this.belowMeanThresholdToolStripMenuItem});
            this.removeSequencesWithFailedReadsToolStripMenuItem.Name = "removeSequencesWithFailedReadsToolStripMenuItem";
            this.removeSequencesWithFailedReadsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.removeSequencesWithFailedReadsToolStripMenuItem.Text = "Remove Sequences";
            // 
            // withFailedReadsToolStripMenuItem
            // 
            this.withFailedReadsToolStripMenuItem.Name = "withFailedReadsToolStripMenuItem";
            this.withFailedReadsToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.withFailedReadsToolStripMenuItem.Text = "With Failed Reads";
            this.withFailedReadsToolStripMenuItem.Click += new System.EventHandler(this.withFailedReadsToolStripMenuItem_Click);
            // 
            // belowMeanThresholdToolStripMenuItem
            // 
            this.belowMeanThresholdToolStripMenuItem.Name = "belowMeanThresholdToolStripMenuItem";
            this.belowMeanThresholdToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
            this.belowMeanThresholdToolStripMenuItem.Text = "Below Mean Threshold...";
            this.belowMeanThresholdToolStripMenuItem.Click += new System.EventHandler(this.belowMeanThresholdToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(218, 6);
            // 
            // removeAdapterSequencesToolStripMenuItem
            // 
            this.removeAdapterSequencesToolStripMenuItem.Name = "removeAdapterSequencesToolStripMenuItem";
            this.removeAdapterSequencesToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.removeAdapterSequencesToolStripMenuItem.Text = "Remove Adapter Sequences";
            this.removeAdapterSequencesToolStripMenuItem.Click += new System.EventHandler(this.removeAdapterSequencesToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(218, 6);
            // 
            // findSequenceToolStripMenuItem
            // 
            this.findSequenceToolStripMenuItem.Name = "findSequenceToolStripMenuItem";
            this.findSequenceToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
            this.findSequenceToolStripMenuItem.Text = "Find Sequence...";
            this.findSequenceToolStripMenuItem.Click += new System.EventHandler(this.findSequenceToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(218, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            this.preferencesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePreferencesToolStripMenuItem});
            this.preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            this.preferencesToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.preferencesToolStripMenuItem.Text = "PREFERENCES";
            // 
            // changePreferencesToolStripMenuItem
            // 
            this.changePreferencesToolStripMenuItem.Name = "changePreferencesToolStripMenuItem";
            this.changePreferencesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.changePreferencesToolStripMenuItem.Text = "Change Preferences...";
            this.changePreferencesToolStripMenuItem.Click += new System.EventHandler(this.changePreferencesToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.helpToolStripMenuItem.Text = "HELP";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(99, 22);
            this.helpToolStripMenuItem1.Text = "Help";
            // 
            // progressStrip
            // 
            this.progressStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.progressStrip.Location = new System.Drawing.Point(0, 590);
            this.progressStrip.Name = "progressStrip";
            this.progressStrip.Size = new System.Drawing.Size(934, 22);
            this.progressStrip.TabIndex = 1;
            this.progressStrip.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // Clean_Sweep_Radio
            // 
            this.Clean_Sweep_Radio.AutoSize = true;
            this.Clean_Sweep_Radio.Checked = true;
            this.Clean_Sweep_Radio.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Clean_Sweep_Radio.Location = new System.Drawing.Point(6, 19);
            this.Clean_Sweep_Radio.Name = "Clean_Sweep_Radio";
            this.Clean_Sweep_Radio.Size = new System.Drawing.Size(99, 19);
            this.Clean_Sweep_Radio.TabIndex = 2;
            this.Clean_Sweep_Radio.TabStop = true;
            this.Clean_Sweep_Radio.Text = "[CLEAN SWEEP]";
            this.Clean_Sweep_Radio.UseVisualStyleBackColor = true;
            this.Clean_Sweep_Radio.CheckedChanged += new System.EventHandler(this.Clean_Sweep_Radio_CheckedChanged);
            // 
            // Decision_Tree_Radio
            // 
            this.Decision_Tree_Radio.AutoSize = true;
            this.Decision_Tree_Radio.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Decision_Tree_Radio.Location = new System.Drawing.Point(111, 19);
            this.Decision_Tree_Radio.Name = "Decision_Tree_Radio";
            this.Decision_Tree_Radio.Size = new System.Drawing.Size(103, 19);
            this.Decision_Tree_Radio.TabIndex = 3;
            this.Decision_Tree_Radio.Text = "[DECISION TREE]";
            this.Decision_Tree_Radio.UseVisualStyleBackColor = true;
            this.Decision_Tree_Radio.CheckedChanged += new System.EventHandler(this.Decision_Tree_Radio_CheckedChanged);
            // 
            // Single_Core_Radio
            // 
            this.Single_Core_Radio.AutoSize = true;
            this.Single_Core_Radio.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.Single_Core_Radio.Location = new System.Drawing.Point(6, 19);
            this.Single_Core_Radio.Name = "Single_Core_Radio";
            this.Single_Core_Radio.Size = new System.Drawing.Size(94, 19);
            this.Single_Core_Radio.TabIndex = 4;
            this.Single_Core_Radio.Text = "[SINGLE CORE]";
            this.Single_Core_Radio.UseVisualStyleBackColor = true;
            this.Single_Core_Radio.CheckedChanged += new System.EventHandler(this.Single_Core_Radio_CheckedChanged);
            // 
            // Multi_Core_Radio
            // 
            this.Multi_Core_Radio.AutoSize = true;
            this.Multi_Core_Radio.Checked = true;
            this.Multi_Core_Radio.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.Multi_Core_Radio.Location = new System.Drawing.Point(106, 19);
            this.Multi_Core_Radio.Name = "Multi_Core_Radio";
            this.Multi_Core_Radio.Size = new System.Drawing.Size(89, 19);
            this.Multi_Core_Radio.TabIndex = 5;
            this.Multi_Core_Radio.TabStop = true;
            this.Multi_Core_Radio.Text = "[MULTI CORE]";
            this.Multi_Core_Radio.UseVisualStyleBackColor = true;
            this.Multi_Core_Radio.CheckedChanged += new System.EventHandler(this.Multi_Core_Radio_CheckedChanged);
            // 
            // Cores_Group_Box
            // 
            this.Cores_Group_Box.Controls.Add(this.Single_Core_Radio);
            this.Cores_Group_Box.Controls.Add(this.Multi_Core_Radio);
            this.Cores_Group_Box.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.Cores_Group_Box.Location = new System.Drawing.Point(12, 540);
            this.Cores_Group_Box.Name = "Cores_Group_Box";
            this.Cores_Group_Box.Size = new System.Drawing.Size(222, 45);
            this.Cores_Group_Box.TabIndex = 6;
            this.Cores_Group_Box.TabStop = false;
            this.Cores_Group_Box.Text = "[LOGICAL CORES]";
            // 
            // Sequencer_Selection_Group_Box
            // 
            this.Sequencer_Selection_Group_Box.Controls.Add(this.Clean_Sweep_Radio);
            this.Sequencer_Selection_Group_Box.Controls.Add(this.Decision_Tree_Radio);
            this.Sequencer_Selection_Group_Box.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.Sequencer_Selection_Group_Box.Location = new System.Drawing.Point(705, 540);
            this.Sequencer_Selection_Group_Box.Name = "Sequencer_Selection_Group_Box";
            this.Sequencer_Selection_Group_Box.Size = new System.Drawing.Size(222, 45);
            this.Sequencer_Selection_Group_Box.TabIndex = 7;
            this.Sequencer_Selection_Group_Box.TabStop = false;
            this.Sequencer_Selection_Group_Box.Text = "[FILE FORMAT]";
            // 
            // FastqGUITabs
            // 
            this.FastqGUITabs.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.FastqGUITabs.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.FastqGUITabs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.FastqGUITabs.Controls.Add(this.InformationTab);
            this.FastqGUITabs.Controls.Add(this.GraphicsTab);
            this.FastqGUITabs.Cursor = System.Windows.Forms.Cursors.Default;
            this.FastqGUITabs.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.FastqGUITabs.ItemSize = new System.Drawing.Size(150, 20);
            this.FastqGUITabs.Location = new System.Drawing.Point(0, 25);
            this.FastqGUITabs.Multiline = true;
            this.FastqGUITabs.Name = "FastqGUITabs";
            this.FastqGUITabs.SelectedIndex = 0;
            this.FastqGUITabs.ShowToolTips = true;
            this.FastqGUITabs.Size = new System.Drawing.Size(934, 515);
            this.FastqGUITabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.FastqGUITabs.TabIndex = 8;
            // 
            // InformationTab
            // 
            this.InformationTab.BackColor = System.Drawing.Color.Transparent;
            this.InformationTab.Controls.Add(this.FastqGUI_Display);
            this.InformationTab.Location = new System.Drawing.Point(24, 4);
            this.InformationTab.Name = "InformationTab";
            this.InformationTab.Padding = new System.Windows.Forms.Padding(3);
            this.InformationTab.Size = new System.Drawing.Size(906, 507);
            this.InformationTab.TabIndex = 0;
            this.InformationTab.Text = "[FASTQ DETAILS]";
            this.InformationTab.ToolTipText = "Shows Fastq file details.";
            // 
            // FastqGUI_Display
            // 
            this.FastqGUI_Display.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FastqGUI_Display.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.FastqGUI_Display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FastqGUI_Display.Cursor = System.Windows.Forms.Cursors.Default;
            this.FastqGUI_Display.Location = new System.Drawing.Point(6, 6);
            this.FastqGUI_Display.Margin = new System.Windows.Forms.Padding(2);
            this.FastqGUI_Display.Name = "FastqGUI_Display";
            this.FastqGUI_Display.ReadOnly = true;
            this.FastqGUI_Display.Size = new System.Drawing.Size(897, 499);
            this.FastqGUI_Display.TabIndex = 9;
            this.FastqGUI_Display.Text = "";
            // 
            // GraphicsTab
            // 
            this.GraphicsTab.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.GraphicsTab.Controls.Add(this.trackBar1);
            this.GraphicsTab.Controls.Add(this.zedGraphControl1);
            this.GraphicsTab.Controls.Add(this.Charts_Combo_Selector);
            this.GraphicsTab.Location = new System.Drawing.Point(24, 4);
            this.GraphicsTab.Name = "GraphicsTab";
            this.GraphicsTab.Padding = new System.Windows.Forms.Padding(3);
            this.GraphicsTab.Size = new System.Drawing.Size(906, 507);
            this.GraphicsTab.TabIndex = 1;
            this.GraphicsTab.Text = "[FASTQ GRAPHICS]";
            this.GraphicsTab.ToolTipText = "Shows Fastq file graphics.";
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(6, 57);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(894, 444);
            this.zedGraphControl1.TabIndex = 2;
            // 
            // Charts_Combo_Selector
            // 
            this.Charts_Combo_Selector.FormattingEnabled = true;
            this.Charts_Combo_Selector.Location = new System.Drawing.Point(611, 6);
            this.Charts_Combo_Selector.MaxLength = 50;
            this.Charts_Combo_Selector.Name = "Charts_Combo_Selector";
            this.Charts_Combo_Selector.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Charts_Combo_Selector.Size = new System.Drawing.Size(284, 23);
            this.Charts_Combo_Selector.TabIndex = 1;
            this.Charts_Combo_Selector.SelectedIndexChanged += new System.EventHandler(this.Charts_Combo_Selector_SelectedIndexChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(6, 6);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(599, 45);
            this.trackBar1.TabIndex = 3;
            // 
            // FastqGUI
            // 
            this.ClientSize = new System.Drawing.Size(934, 612);
            this.Controls.Add(this.FastqGUITabs);
            this.Controls.Add(this.Sequencer_Selection_Group_Box);
            this.Controls.Add(this.Cores_Group_Box);
            this.Controls.Add(this.progressStrip);
            this.Controls.Add(this.MenuStrip);
            this.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.MaximizeBox = false;
            this.Name = "FastqGUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "[FASTQ-ANALYZER-CLEANER]";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.progressStrip.ResumeLayout(false);
            this.progressStrip.PerformLayout();
            this.Cores_Group_Box.ResumeLayout(false);
            this.Cores_Group_Box.PerformLayout();
            this.Sequencer_Selection_Group_Box.ResumeLayout(false);
            this.Sequencer_Selection_Group_Box.PerformLayout();
            this.FastqGUITabs.ResumeLayout(false);
            this.InformationTab.ResumeLayout(false);
            this.GraphicsTab.ResumeLayout(false);
            this.GraphicsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OpenFastqDialogue;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFastqToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveFastqToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.StatusStrip progressStrip;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clean3EndsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clean5EndsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanTailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem actionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reanalyzeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSequenceStatisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem preferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePreferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.RadioButton Clean_Sweep_Radio;
        private System.Windows.Forms.RadioButton Decision_Tree_Radio;
        private System.Windows.Forms.RadioButton Single_Core_Radio;
        private System.Windows.Forms.RadioButton Multi_Core_Radio;
        private System.Windows.Forms.GroupBox Cores_Group_Box;
        private System.Windows.Forms.GroupBox Sequencer_Selection_Group_Box;
        private System.Windows.Forms.TabControl FastqGUITabs;
        private System.Windows.Forms.TabPage InformationTab;
        private System.Windows.Forms.TabPage GraphicsTab;
        private FastqGUI_Charts FastqGUI_Charts;
        private System.Windows.Forms.ComboBox Charts_Combo_Selector;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem removeAdapterSequencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCSVDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem findSequenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem flushMemoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem removeSequencesWithFailedReadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem withFailedReadsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem belowMeanThresholdToolStripMenuItem;
        private System.Windows.Forms.RichTextBox FastqGUI_Display;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}

