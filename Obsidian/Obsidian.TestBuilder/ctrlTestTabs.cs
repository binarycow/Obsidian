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

namespace Obsidian.TestBuilder
{
    public partial class ctrlTestTabs : UserControl
    {
        public ctrlTestTabs()
        {
            InitializeComponent();
        }

        public void SetTest(Test test)
        {
            ctrlProperties.Test = test;
        }
    }
}
