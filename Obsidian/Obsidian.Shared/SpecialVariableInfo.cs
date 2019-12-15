//using Common.ExpressionCreators;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using StringBuilder = System.Text.StringBuilder;

//namespace Obsidian
//{
//    internal class SpecialVariableInfo
//    {
//        internal class AssignInfo<T> : AssignInfo
//        {
//            internal AssignInfo(ExpressionExtensionData<T> extensionData, BinaryExpression assignmentExpression) : base(extensionData, assignmentExpression)
//            {
//                TypedExtensionData = extensionData;
//            }
//            internal ExpressionExtensionData<T> TypedExtensionData { get; }
//        }
//        internal class AssignInfo
//        {
//            internal AssignInfo(ExpressionExtensionData extensionData, BinaryExpression assignmentExpression)
//            {
//                ExtensionData = extensionData;
//                AssignmentExpression = assignmentExpression;
//            }
//            internal ExpressionExtensionData ExtensionData { get; }
//            internal BinaryExpression AssignmentExpression { get; }
//        }

//        internal SpecialVariableInfo(AssignInfo<StringBuilder> stringBuilder, AssignInfo<Self> self)
//        {
//            _StringBuilder = stringBuilder;
//            _Self = self;
//        }

//        private AssignInfo<StringBuilder> _StringBuilder;
//        private AssignInfo<Self> _Self;

//        internal ExpressionExtensionData<StringBuilder> StringBuilder => _StringBuilder.TypedExtensionData;
//        internal ExpressionExtensionData<Self> Self => _Self.TypedExtensionData;

//        internal IEnumerable<AssignInfo> AllAssignInfo => new AssignInfo[]
//        {
//                _StringBuilder,
//                _Self,
//        };

//        internal IEnumerable<ParameterExpression> AllVariables => AllAssignInfo.Select(x => x.ExtensionData.ParameterExpression);
//        internal IEnumerable<BinaryExpression> AllAssignments => AllAssignInfo.Select(x => x.AssignmentExpression);
//    }

//}
