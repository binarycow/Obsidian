using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obsidian.TestCore
{
    public class Category : Item
    {
        private string _CategoryName;

        public override Item this[string name] 
        {
            get => Children.FirstOrDefault(child => child.Name == name);
        }

        public string CategoryName
        {
            get => _CategoryName;
            set => SetField(ref _CategoryName, value);
        }
        public ObservableCollection<Item> Children { get; set; }

        public override string Name => CategoryName;
    }
}
