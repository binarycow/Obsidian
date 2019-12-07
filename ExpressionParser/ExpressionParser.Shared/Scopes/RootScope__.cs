//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using ExpressionParser.VariableManagement;

//namespace ExpressionParser.Scopes
//{
//    internal class RootScope : Scope
//    {
//        private RootScope(IDictionary<string, object?> variables) : base(null)
//        {
//            foreach(var name in variables.Keys)
//            {
//                _GlobalVariableNames.Add(name);
//                _GlobalVariableTypes.Add(variables[name]?.GetType() ?? typeof(object));
//            }
//        }

//        protected int NextIndex => _GlobalVariableNames.Count;
//        private List<string> _GlobalVariableNames { get; } = new List<string>();
//        private List<Type> _GlobalVariableTypes { get; } = new List<Type>();

//        public override Scope FindRootScope()
//        {
//            return this;
//        }

//        public override bool TryGetVariable(string variableName, [NotNullWhen(true)] out Expression? expression)
//        {
//            if (base.TryGetVariable(variableName, out expression) == true)
//            {
//                return true;
//            }
//            if(_GlobalVariableNames.TryGetIndex(variableName, out var index) == false)
//            {
//                return false;
//            }
//            expression = Expression.ArrayIndex(RootParameterExpression, Expression.Constant(index));
//            expression = Expression.Convert(expression, _GlobalVariableTypes[index]);
//            return true;
//        }

//        public VariableInfo[] GetVariableInfo()
//        {
//            return Enumerable.Range(0, _GlobalVariableNames.Count).Select(index => 
//                new VariableInfo(_GlobalVariableNames[index], _GlobalVariableTypes[index], index)
//            ).ToArray();
//        }

//        public override bool IsRoot => true;

//        public ParameterExpression RootParameterExpression { get; } = Expression.Parameter(typeof(object?[]), "args");


//        protected override IDictionary<string, Expression> VariableWalk(IDictionary<string, Expression> dict)
//        {
//            foreach (var key in _GlobalVariableNames)
//            {
//                if (dict.ContainsKey(key) == false && TryGetVariable(key, out var expression))
//                {
//                    dict.Add(key, expression);
//                }
//            }
//            return dict;
//        }


//        public static RootScope CreateRootScope(IDictionary<string, object?> variables)
//        {
//            return new RootScope(variables);
//        }

//    }
//}
