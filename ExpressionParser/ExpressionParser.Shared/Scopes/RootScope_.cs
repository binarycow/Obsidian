//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace ExpressionParser.Scopes
//{
//    internal class RootScope : Scope
//    {
//        private RootScope(string name) : base(null, name) { }
//        private RootScope() : base(null, null) { }


//        private List<BinaryExpression> _AssignExpressions = new List<BinaryExpression>();

//        internal virtual ParameterExpression AddLocalVariable(string name, Expression assignValue)
//        {
//            return AddLocalVariable(name, assignValue, out _);
//        }
//        internal override ParameterExpression AddLocalVariable(string name, Expression assignValue, out BinaryExpression assignExpression)
//        {
//            var variable = Expression.Variable(assignValue.Type, name);
//            base.AddLocalVariable(name, assignValue, out assignExpression);
//            _AssignExpressions.Add(assignExpression);
//            return variable;
//        }


//        internal static RootScope CreateRootScope(IDictionary<string, object?> variables)
//        {
//            var ret = new RootScope();
//            foreach (var varName in variables.Keys)
//            {
//                ret.AddParameter(varName, Expression.Constant(variables[varName]), out var assignExpression);
//                ret._AssignExpressions.Add(assignExpression);
//            }
//            return ret;
//        }
//    }
//}
