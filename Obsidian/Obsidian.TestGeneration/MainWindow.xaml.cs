using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace Obsidian.TestGeneration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string savePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Templating.Tests", "testData.json");
        ObservableCollection <Category> tests = new ObservableCollection<Category>();
        public MainWindow()
        {
            InitializeComponent();
            var fileContents = File.ReadAllText(savePath);
            var testData = JsonConvert.DeserializeObject<Category[]>(fileContents);
            tests = new ObservableCollection<Category>(testData);
            this.DataContext = tests;
        }

    }
}
