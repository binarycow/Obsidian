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
    internal partial class ctrlTestTabs : UserControl
    {
        internal ctrlTestTabs()
        {
            InitializeComponent();
        }

        internal void SetTest(Test test)
        {
            ctrlProperties.Test = test;
        }
    }
}
