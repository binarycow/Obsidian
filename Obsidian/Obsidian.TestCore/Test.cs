using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obsidian.TestCore
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    public class Test : Item
    {
        private string _TestName = string.Empty;
        public string TestName
        {
            get => _TestName;
            set
            {
                SetField(ref _TestName, value);
                OnPropertyChanged(nameof(Name));
            }
        }
        private string _RootPath = string.Empty;
        public string RootPath
        {
            get => _RootPath;
            set => SetField(ref _RootPath, value);
        }
        private string _InputFile = string.Empty;
        public string InputFile
        {
            get => _InputFile;
            set => SetField(ref _InputFile, value);
        }
        private string _ExpectedFile = string.Empty;
        public string ExpectedFile
        {
            get => _ExpectedFile;
            set => SetField(ref _ExpectedFile, value);
        }
        private string _ActualFile = string.Empty;
        public string ActualFile
        {
            get => _ActualFile;
            set => SetField(ref _ActualFile, value);
        }
        private string _VariablesFile = string.Empty;
        public string VariablesFile
        {
            get => _VariablesFile;
            set => SetField(ref _VariablesFile, value);
        }

        private bool _trim_blocks;

        public bool trim_blocks
        {
            get => _trim_blocks;
            set => SetField(ref _trim_blocks, value);
        }
        private bool _lstrip_blocks;
        public bool lstrip_blocks
        {
            get => _lstrip_blocks;
            set => SetField(ref _lstrip_blocks, value);
        }

        public override string Name
        {
            get => TestName;
            set => TestName = value;
        }

        public override Item this[string name] => throw new InvalidOperationException();
    }
}
