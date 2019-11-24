using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JsonSubTypes;
using Newtonsoft.Json;

namespace Obsidian.TestGeneration
{
    [JsonConverter(typeof(TestItem), "Kind")]
    //[JsonSubtypes.KnownSubType(typeof(Category), "Category")]
    //[JsonSubtypes.KnownSubType(typeof(Test), "Test")]
    public abstract class TestItem : INotifyPropertyChanged
    {
        public TestItem()
        {

        }

        public abstract string Kind { get; }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }
}
