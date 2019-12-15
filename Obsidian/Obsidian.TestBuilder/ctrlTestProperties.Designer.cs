using System.Windows.Forms;

namespace Obsidian.TestBuilder
{
    partial class ctrlTestProperties
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtRootPath = new System.Windows.Forms.TextBox();
            this.testBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnBrowseRootPath = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.chkTrimBlocks = new System.Windows.Forms.CheckBox();
            this.chkLStripBlocks = new System.Windows.Forms.CheckBox();
            this.txtInputFilename = new System.Windows.Forms.TextBox();
            this.txtExpectedFilename = new System.Windows.Forms.TextBox();
            this.txtActualFilename = new System.Windows.Forms.TextBox();
            this.txtVariablesFilename = new System.Windows.Forms.TextBox();
            this.btnBrowseVariables = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testBindingSource)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtRootPath, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnBrowseRootPath, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.txtInputFilename, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtExpectedFilename, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.txtActualFilename, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.txtVariablesFilename, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.btnBrowseVariables, 2, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(577, 463);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtRootPath
            // 
            this.txtRootPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.testBindingSource, "RootPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtRootPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRootPath.Location = new System.Drawing.Point(109, 29);
            this.txtRootPath.Name = "txtRootPath";
            this.txtRootPath.Size = new System.Drawing.Size(384, 20);
            this.txtRootPath.TabIndex = 3;
            // 
            // testBindingSource
            // 
            this.testBindingSource.DataSource = typeof(Obsidian.TestCore.Test);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Root Path:";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // txtName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtName, 2);
            this.txtName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.testBindingSource, "TestName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(109, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(465, 20);
            this.txtName.TabIndex = 1;
            // 
            // btnBrowseRootPath
            // 
            this.btnBrowseRootPath.Location = new System.Drawing.Point(499, 29);
            this.btnBrowseRootPath.Name = "btnBrowseRootPath";
            this.btnBrowseRootPath.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseRootPath.TabIndex = 4;
            this.btnBrowseRootPath.Text = "Browse...";
            this.btnBrowseRootPath.UseVisualStyleBackColor = true;
            this.btnBrowseRootPath.Click += new System.EventHandler(this.BtnBrowseRootPath_Click);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Input Filename:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Expected Filename:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Actual Filename:";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(98, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Variables Filename:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 3);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.chkTrimBlocks, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.chkLStripBlocks, 3, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 165);
            this.tableLayoutPanel2.MinimumSize = new System.Drawing.Size(0, 26);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(571, 26);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // chkTrimBlocks
            // 
            this.chkTrimBlocks.AutoSize = true;
            this.chkTrimBlocks.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.testBindingSource, "trim_blocks", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkTrimBlocks.Location = new System.Drawing.Point(168, 3);
            this.chkTrimBlocks.Name = "chkTrimBlocks";
            this.chkTrimBlocks.Size = new System.Drawing.Size(81, 17);
            this.chkTrimBlocks.TabIndex = 0;
            this.chkTrimBlocks.Text = "Trim Blocks";
            this.chkTrimBlocks.UseVisualStyleBackColor = true;
            // 
            // chkLStripBlocks
            // 
            this.chkLStripBlocks.AutoSize = true;
            this.chkLStripBlocks.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.testBindingSource, "lstrip_blocks", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkLStripBlocks.Location = new System.Drawing.Point(315, 3);
            this.chkLStripBlocks.Name = "chkLStripBlocks";
            this.chkLStripBlocks.Size = new System.Drawing.Size(88, 17);
            this.chkLStripBlocks.TabIndex = 1;
            this.chkLStripBlocks.Text = "LStrip Blocks";
            this.chkLStripBlocks.UseVisualStyleBackColor = true;
            // 
            // txtInputFilename
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtInputFilename, 2);
            this.txtInputFilename.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.testBindingSource, "InputFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtInputFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInputFilename.Location = new System.Drawing.Point(109, 58);
            this.txtInputFilename.Name = "txtInputFilename";
            this.txtInputFilename.Size = new System.Drawing.Size(465, 20);
            this.txtInputFilename.TabIndex = 10;
            // 
            // txtExpectedFilename
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtExpectedFilename, 2);
            this.txtExpectedFilename.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.testBindingSource, "ExpectedFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtExpectedFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExpectedFilename.Location = new System.Drawing.Point(109, 84);
            this.txtExpectedFilename.Name = "txtExpectedFilename";
            this.txtExpectedFilename.Size = new System.Drawing.Size(465, 20);
            this.txtExpectedFilename.TabIndex = 11;
            // 
            // txtActualFilename
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.txtActualFilename, 2);
            this.txtActualFilename.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.testBindingSource, "ActualFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtActualFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtActualFilename.Location = new System.Drawing.Point(109, 110);
            this.txtActualFilename.Name = "txtActualFilename";
            this.txtActualFilename.Size = new System.Drawing.Size(465, 20);
            this.txtActualFilename.TabIndex = 12;
            // 
            // txtVariablesFilename
            // 
            this.txtVariablesFilename.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.testBindingSource, "VariablesFile", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtVariablesFilename.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVariablesFilename.Location = new System.Drawing.Point(109, 136);
            this.txtVariablesFilename.Name = "txtVariablesFilename";
            this.txtVariablesFilename.Size = new System.Drawing.Size(384, 20);
            this.txtVariablesFilename.TabIndex = 13;
            // 
            // btnBrowseVariables
            // 
            this.btnBrowseVariables.Location = new System.Drawing.Point(499, 136);
            this.btnBrowseVariables.Name = "btnBrowseVariables";
            this.btnBrowseVariables.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseVariables.TabIndex = 14;
            this.btnBrowseVariables.Text = "Browse...";
            this.btnBrowseVariables.UseVisualStyleBackColor = true;
            this.btnBrowseVariables.Click += new System.EventHandler(this.BtnBrowseVariables_Click);
            // 
            // ctrlTestProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ctrlTestProperties";
            this.Size = new System.Drawing.Size(577, 463);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testBindingSource)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.BindingSource testBindingSource;
        private System.Windows.Forms.TextBox txtRootPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseRootPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox chkTrimBlocks;
        private System.Windows.Forms.CheckBox chkLStripBlocks;
        private System.Windows.Forms.TextBox txtInputFilename;
        private System.Windows.Forms.TextBox txtExpectedFilename;
        private System.Windows.Forms.TextBox txtActualFilename;
        private System.Windows.Forms.TextBox txtVariablesFilename;
        private System.Windows.Forms.Button btnBrowseVariables;
    }
}
