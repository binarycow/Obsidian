using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Obsidian.TestCore;

namespace Obsidian.TestBuilder
{
    public partial class Form1 : Form
    {
        public static string RootPath = Path.GetFullPath(Path.Combine(Application.ExecutablePath, "..", "..", "..", "..", "TestData"));

        private ctrlTestTabs testTabs = new ctrlTestTabs() { Visible = false, Dock = DockStyle.Fill };

        private Dictionary<Item, TreeNode> _Nodes = new Dictionary<Item, TreeNode>();

        public Form1()
        {
            InitializeComponent();
            Reset(false);


            splitContainer1.Panel2.Controls.Add(testTabs);
        }




        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TestRunner.Save("Tests.json");
                MessageBox.Show("Save Complete.");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Couldn't save: {Environment.NewLine}{ex.GetType().Name}{Environment.NewLine}{ex.Message}");
            }
        }

        private void resetFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
        }

        void Reset(bool messageBox = true)
        {

            try
            {
                TestRunner.Init("Tests.json", RootPath);
                treeView1.Nodes.Clear();
                _Nodes.Clear();

                foreach (var item in TestRunner.TestItems.Values)
                {
                    treeView1.Nodes.Add(CreateNode(item));
                }
                treeView1.ExpandAll();
                if(messageBox)
                {
                    MessageBox.Show("Reset Complete.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Couldn't reset: {Environment.NewLine}{ex.GetType().Name}{Environment.NewLine}{ex.Message}");
            }
        }

        private TreeNode CreateNode(Item item)
        {
            switch(item)
            {
                case Test test:
                    return CreateTestNode(test);
                case Category category:
                    return CreateCategoryNode(category);
                default:
                    throw new NotImplementedException();
            }
        }
        private TreeNode CreateTestNode(Test item)
        {
            var treeNode = new TreeNode($"Test: {item.Name}");
            treeNode.Tag = item;
            item.PropertyChanged += Test_PropertyChanged;
            _Nodes.Add(item, treeNode);
            return treeNode;
        }
        private TreeNode CreateCategoryNode(Category item)
        {
            var treeNode = new TreeNode($"Category: {item.Name}");
            treeNode.Tag = item;
            foreach(var child in item.Children)
            {
                treeNode.Nodes.Add(CreateNode(child));
            }
            item.PropertyChanged += Category_PropertyChanged;
            _Nodes.Add(item, treeNode);
            return treeNode;
        }

        private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is Category category)) return;
            if (_Nodes.TryGetValue(category, out var treeNode) == false) return;

            switch(e.PropertyName)
            {
                case nameof(Category.Name):
                    treeNode.Text = $"Category: {category.Name}";
                    break;
            }
        }
        private void Test_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is Test test)) return;
            if (_Nodes.TryGetValue(test, out var treeNode) == false) return;

            switch (e.PropertyName)
            {
                case nameof(Test.Name):
                    treeNode.Text = $"Test: {test.Name}";
                    break;
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch(e.Node.Tag)
            {
                case Test test:
                    testTabs.Visible = true;
                    testTabs.SetTest(test);
                    break;
                case Category category:
                    testTabs.Visible = false;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void runTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var test = ((sender as ToolStripMenuItem)?.GetCurrentParent() as ContextMenuStrip)?.Tag as Test;
            if (test == default) throw new NotImplementedException();
            TestRunner.TestTemplate(test, out string actualOutput, out string expectedOutput);
            testTabs.txtActualOutput.Text = actualOutput.Replace("\n", Environment.NewLine);
            testTabs.txtExpectedOutput.Text = expectedOutput.Replace("\n", Environment.NewLine);
            testTabs.tabControl1.SelectTab(testTabs.tabTestResults);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            // Make sure this is the right button.
            if (e.Button != MouseButtons.Right) return;

            // Select this node.
            TreeNode node_here = treeView1.GetNodeAt(e.X, e.Y);
            treeView1.SelectedNode = node_here;

            // See if we got a node.
            if (node_here == null) return;


            switch(node_here.Tag)
            {
                case Test test:
                    contextMenuTest.Tag = test;
                    contextMenuTest.Show(treeView1, new Point(e.X, e.Y));
                    break;
                case Category category:
                    contextMenuCategory.Tag = category;
                    contextMenuCategory.Show(treeView1, new Point(e.X, e.Y));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void addTestToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
