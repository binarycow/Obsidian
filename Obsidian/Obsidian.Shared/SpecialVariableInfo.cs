//using Common.ExpressionCreators;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using StringBuilder = System.Text.StringBuilder;

//namespace Obsidian
//{
//    public class SpecialVariableInfo
//    {
//        public class AssignInfo<T> : AssignInfo
//        {
//            public AssignInfo(ExpressionExtensionData<T> extensionData, BinaryExpression assignmentExpression) : base(extensionData, assignmentExpression)
//            {
//                TypedExtensionData = extensionData;
//            }
//            public ExpressionExtensionData<T> TypedExtensionData { get; }
//        }
//        public class AssignInfo
//        {
//            public AssignInfo(ExpressionExtensionData extensionData, BinaryExpression assignmentExpression)
//            {
//                ExtensionData = extensionData;
//                AssignmentExpression = assignmentExpression;
//            }
//            public ExpressionExtensionData ExtensionData { get; }
//            public BinaryExpression AssignmentExpression { get; }
//        }

//        public SpecialVariableInfo(AssignInfo<StringBuilder> stringBuilder, AssignInfo<Self> self)
//        {
//            _StringBuilder = stringBuilder;
//            _Self = self;
//        }

//        private AssignInfo<StringBuilder> _StringBuilder;
//        private AssignInfo<Self> _Self;

//        public ExpressionExtensionData<StringBuilder> StringBuilder => _StringBuilder.TypedExtensionData;
//        public ExpressionExtensionData<Self> Self => _Self.TypedExtensionData;

//        public IEnumerable<AssignInfo> AllAssignInfo => new AssignInfo[]
//        {
//                _StringBuilder,
//                _Self,
//        };

//        public IEnumerable<ParameterExpression> AllVariables => AllAssignInfo.Select(x => x.ExtensionData.ParameterExpression);
//        public IEnumerable<BinaryExpression> AllAssignments => AllAssignInfo.Select(x => x.AssignmentExpression);
//    }

//}
