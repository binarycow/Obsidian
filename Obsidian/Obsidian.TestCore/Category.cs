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

        public override Item this[string name] 
        {
            get => Children.FirstOrDefault(child => child.Name == name);
        }

        private string _CategoryName = string.Empty;
        public string CategoryName
        {
            get => _CategoryName;
            set
            {
                SetField(ref _CategoryName, value);
                OnPropertyChanged(nameof(Name));
            }
        }

        public ObservableCollection<Item> Children { get; set; } = new ObservableCollection<Item>();

        public override string Name
        {
            get => CategoryName;
            set => CategoryName = value;
        }
    }
}
