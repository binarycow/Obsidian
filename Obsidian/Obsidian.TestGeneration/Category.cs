using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obsidian.TestGeneration
{
    public class Category : TestItem
    {
        public Category()
        {
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Children));
        }

        private string _CategoryName;
        public string CategoryName
        {
            get => _CategoryName;
            set => SetField(ref _CategoryName, value);
        }

        public override string Kind { get; } = "Category";

        public ObservableCollection<TestItem> Children { get; set; }
    }
}
