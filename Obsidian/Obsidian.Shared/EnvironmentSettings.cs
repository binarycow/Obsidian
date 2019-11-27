using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Obsidian
{
    public class EnvironmentSettings
    {

        private bool _TrimBlocks = false;
        public bool TrimBlocks
        {
            get { return _TrimBlocks; }
            set { TrySetValue(ref _TrimBlocks, value); }
        }
        private bool _LStripBlocks = false;
        public bool LStripBlocks
        {
            get { return _LStripBlocks; }
            set { TrySetValue(ref _LStripBlocks, value); }
        }
        private bool _TrimTrailingNewLine = true;
        public bool TrimTrailingNewline
        {
            get { return _TrimTrailingNewLine; }
            set { TrySetValue(ref _TrimTrailingNewLine, value); }
        }
        private string _BlockStartString = "{%";
        public string BlockStartString
        {
            get { return _BlockStartString; }
            set { TrySetValue(ref _BlockStartString, value); }
        }
        private string _BlockEndString = "%}";
        public string BlockEndString
        {
            get { return _BlockEndString; }
            set { TrySetValue(ref _BlockEndString, value); }
        }
        private string _VariableStartString = "{{";
        public string VariableStartString
        {
            get { return _VariableStartString; }
            set { TrySetValue(ref _VariableStartString, value); }
        }
        private string _VariableEndString = "}}";
        public string VariableEndString
        {
            get { return _VariableEndString; }
            set { TrySetValue(ref _VariableEndString, value); }
        }
        private string _CommentStartString = "{#";
        public string CommentStartString
        {
            get { return _CommentStartString; }
            set { TrySetValue(ref _CommentStartString, value); }
        }
        private string _CommentEndString = "#}";
        public string CommentEndString
        {
            get { return _CommentEndString; }
            set { TrySetValue(ref _CommentEndString, value); }
        }
        private string? _LineStatementPrefix = null;
        public string? LineStatementPrefix
        {
            get { return _LineStatementPrefix; }
            set { TrySetValue(ref _LineStatementPrefix, value); }
        }
        private string? _LineCommentPrefix = null;
        public string? LineCommentPrefix
        {
            get { return _LineCommentPrefix; }
            set { TrySetValue(ref _LineCommentPrefix, value); }
        }
        private bool _TreatNullCollectionsAsEmpty = true;
        public bool TreatNullCollectionsAsEmpty
        {
            get { return _TreatNullCollectionsAsEmpty; }
            set { TrySetValue(ref _TreatNullCollectionsAsEmpty, value); }
        }
        private bool _DynamicTemplates = true;
        public bool DynamicTemplates
        {
            get { return _DynamicTemplates; }
            set { TrySetValue(ref _DynamicTemplates, value); }
        }


        private bool _ReadOnly;
        public bool IsReadOnly
        {
            get => _ReadOnly;
            set
            {
                if (_ReadOnly == true && value == false)
                {
                    throw new InvalidOperationException($"{nameof(EnvironmentSettings)}.{nameof(IsReadOnly)} has already been set to {true}.");
                }
                _ReadOnly = value;
            }
        }

        private void TrySetValue<T>(ref T field, T value)
        {
            if (IsReadOnly)
            {
                throw new InvalidOperationException($"{nameof(EnvironmentSettings)}.{nameof(IsReadOnly)} is set to {true}.");
            }
            field = value;
        }
    }

}
