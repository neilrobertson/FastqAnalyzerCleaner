namespace FastqAnalyzerCleaner
{
    partial class Form1
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFastqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveFastqToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFastaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clean3EndsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clean5EndsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanTailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.createFastaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aCTIONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rescanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reanalyzeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSequenceStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pREFERENCESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changePreferencesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hELPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.menuStrip1.SuspendLayout();
            this.progressStrip.SuspendLayout();
            this.Cores_Group_Box.SuspendLayout();
            this.Sequencer_Selection_Group_Box.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.aCTIONToolStripMenuItem,
            this.pREFERENCESToolStripMenuItem,
            this.hELPToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(834, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFastqToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveFastqToolStripMenuItem,
            this.saveFastaToolStripMenuItem,
            this.toolStripSeparator2,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.fileToolStripMenuItem.Text = "FILE";
            // 
            // openFastqToolStripMenuItem
            // 
            this.openFastqToolStripMenuItem.Name = "openFastqToolStripMenuItem";
            this.openFastqToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.openFastqToolStripMenuItem.Text = "Open Fastq...";
            this.openFastqToolStripMenuItem.Click += new System.EventHandler(this.openFastqFile);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // saveFastqToolStripMenuItem
            // 
            this.saveFastqToolStripMenuItem.Name = "saveFastqToolStripMenuItem";
            this.saveFastqToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.saveFastqToolStripMenuItem.Text = "Save Fastq...";
            this.saveFastqToolStripMenuItem.Click += new System.EventHandler(this.saveFastqToolStripMenuItem_Click);
            // 
            // saveFastaToolStripMenuItem
            // 
            this.saveFastaToolStripMenuItem.Name = "saveFastaToolStripMenuItem";
            this.saveFastaToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.saveFastaToolStripMenuItem.Text = "Save Fasta...";
            this.saveFastaToolStripMenuItem.Click += new System.EventHandler(this.saveFastaToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(140, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clean3EndsToolStripMenuItem,
            this.clean5EndsToolStripMenuItem,
            this.cleanTailsToolStripMenuItem,
            this.toolStripSeparator3,
            this.createFastaToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.editToolStripMenuItem.Text = "EDIT";
            // 
            // clean3EndsToolStripMenuItem
            // 
            this.clean3EndsToolStripMenuItem.Name = "clean3EndsToolStripMenuItem";
            this.clean3EndsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.clean3EndsToolStripMenuItem.Text = "Clean 3\' Ends";
            this.clean3EndsToolStripMenuItem.Click += new System.EventHandler(this.clean3EndsToolStripMenuItem_Click);
            // 
            // clean5EndsToolStripMenuItem
            // 
            this.clean5EndsToolStripMenuItem.Name = "clean5EndsToolStripMenuItem";
            this.clean5EndsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.clean5EndsToolStripMenuItem.Text = "Clean 5\' Ends";
            this.clean5EndsToolStripMenuItem.Click += new System.EventHandler(this.clean5EndsToolStripMenuItem_Click);
            // 
            // cleanTailsToolStripMenuItem
            // 
            this.cleanTailsToolStripMenuItem.Name = "cleanTailsToolStripMenuItem";
            this.cleanTailsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.cleanTailsToolStripMenuItem.Text = "Clean Tails";
            this.cleanTailsToolStripMenuItem.Click += new System.EventHandler(this.cleanTailsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(144, 6);
            // 
            // createFastaToolStripMenuItem
            // 
            this.createFastaToolStripMenuItem.Name = "createFastaToolStripMenuItem";
            this.createFastaToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.createFastaToolStripMenuItem.Text = "Create Fasta...";
            this.createFastaToolStripMenuItem.Click += new System.EventHandler(this.createFastaToolStripMenuItem_Click);
            // 
            // aCTIONToolStripMenuItem
            // 
            this.aCTIONToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rescanToolStripMenuItem,
            this.reanalyzeToolStripMenuItem,
            this.showSequenceStatisticsToolStripMenuItem});
            this.aCTIONToolStripMenuItem.Name = "aCTIONToolStripMenuItem";
            this.aCTIONToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.aCTIONToolStripMenuItem.Text = "ACTION";
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
            // pREFERENCESToolStripMenuItem
            // 
            this.pREFERENCESToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changePreferencesToolStripMenuItem});
            this.pREFERENCESToolStripMenuItem.Name = "pREFERENCESToolStripMenuItem";
            this.pREFERENCESToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.pREFERENCESToolStripMenuItem.Text = "PREFERENCES";
            // 
            // changePreferencesToolStripMenuItem
            // 
            this.changePreferencesToolStripMenuItem.Name = "changePreferencesToolStripMenuItem";
            this.changePreferencesToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.changePreferencesToolStripMenuItem.Text = "Change Preferences...";
            // 
            // hELPToolStripMenuItem
            // 
            this.hELPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem1});
            this.hELPToolStripMenuItem.Name = "hELPToolStripMenuItem";
            this.hELPToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.hELPToolStripMenuItem.Text = "HELP";
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
            this.progressStrip.Location = new System.Drawing.Point(0, 440);
            this.progressStrip.Name = "progressStrip";
            this.progressStrip.Size = new System.Drawing.Size(834, 22);
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
            this.Cores_Group_Box.Location = new System.Drawing.Point(12, 395);
            this.Cores_Group_Box.Name = "Cores_Group_Box";
            this.Cores_Group_Box.Size = new System.Drawing.Size(214, 42);
            this.Cores_Group_Box.TabIndex = 6;
            this.Cores_Group_Box.TabStop = false;
            this.Cores_Group_Box.Text = "[LOGICAL CORES]";
            // 
            // Sequencer_Selection_Group_Box
            // 
            this.Sequencer_Selection_Group_Box.Controls.Add(this.Clean_Sweep_Radio);
            this.Sequencer_Selection_Group_Box.Controls.Add(this.Decision_Tree_Radio);
            this.Sequencer_Selection_Group_Box.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F);
            this.Sequencer_Selection_Group_Box.Location = new System.Drawing.Point(600, 392);
            this.Sequencer_Selection_Group_Box.Name = "Sequencer_Selection_Group_Box";
            this.Sequencer_Selection_Group_Box.Size = new System.Drawing.Size(222, 45);
            this.Sequencer_Selection_Group_Box.TabIndex = 7;
            this.Sequencer_Selection_Group_Box.TabStop = false;
            this.Sequencer_Selection_Group_Box.Text = "[SEQUENCER SELECTION]";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(834, 462);
            this.Controls.Add(this.Sequencer_Selection_Group_Box);
            this.Controls.Add(this.Cores_Group_Box);
            this.Controls.Add(this.progressStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.progressStrip.ResumeLayout(false);
            this.progressStrip.PerformLayout();
            this.Cores_Group_Box.ResumeLayout(false);
            this.Cores_Group_Box.PerformLayout();
            this.Sequencer_Selection_Group_Box.ResumeLayout(false);
            this.Sequencer_Selection_Group_Box.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFastqToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveFastqToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveFastaToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem createFastaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aCTIONToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rescanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reanalyzeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSequenceStatisticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pREFERENCESToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changePreferencesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hELPToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.RadioButton Clean_Sweep_Radio;
        private System.Windows.Forms.RadioButton Decision_Tree_Radio;
        private System.Windows.Forms.RadioButton Single_Core_Radio;
        private System.Windows.Forms.RadioButton Multi_Core_Radio;
        private System.Windows.Forms.GroupBox Cores_Group_Box;
        private System.Windows.Forms.GroupBox Sequencer_Selection_Group_Box;
    }
}

