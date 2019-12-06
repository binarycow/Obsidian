using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Obsidian.TestCore;
using System.IO;

namespace Obsidian.TestBuilder
{
    public partial class ctrlTestProperties : UserControl
    {
        public ctrlTestProperties()
        {
            InitializeComponent();
        }


        private Test _Test;

        public Test Test
        {
            get => _Test; 
            set 
            { 
                _Test = value;
                testBindingSource.DataSource = value;
            }
        }

        private void btnBrowseRootPath_Click(object sender, EventArgs e)
        {
            using var fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Path.Combine(Form1.RootPath, Test.RootPath);
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                Test.RootPath = fbd.SelectedPath.Replace(Form1.RootPath, "").TrimStart('\\');
            }
        }

        private void btnBrowseVariables_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog();
            sfd.InitialDirectory = Path.Combine(Form1.RootPath, Test.RootPath);
            sfd.FileName = Test.VariablesFile;
            sfd.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Test.VariablesFile = sfd.FileName.Replace(Form1.RootPath, "").Replace(Test.RootPath, "").TrimStart('\\');
            }
        }
    }
}
