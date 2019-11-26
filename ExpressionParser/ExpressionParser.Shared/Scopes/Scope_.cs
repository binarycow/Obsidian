//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using Common.ExpressionCreators;
//using ExpressionParser.VariableManagement;

//namespace ExpressionParser.Scopes
//{
//    public class Scope
//    {
//        protected Scope(Scope? parentScope, string? name = null)
//        {
//            ParentScope = parentScope;
//            Name = name;
//        }

//        private Scope? ParentScope { get; }
//        private Dictionary<string, ParameterExpression> _LocalVariables = new Dictionary<string, ParameterExpression>();
//        public IEnumerable<ParameterExpression> LocalVariables => _LocalVariables.Values;
//        private Dictionary<string, ParameterExpression> _InternalVariables = new Dictionary<string, ParameterExpression>();
//        public IEnumerable<ParameterExpression> InternalVariables => _InternalVariables.Values;
//        public IEnumerable<ParameterExpression> AllVariables => InternalVariables.Concat(LocalVariables);
//        public List<ParameterExpression> Parameters { get; } = new List<ParameterExpression>();

//        public BlockExpression CloseScope(Expression[] body, bool includeParameters = false, bool includeInternalVariables = false)
//        {
//            var variables = LocalVariables;
//            if (includeInternalVariables)
//            {
//                variables = variables.Concat(InternalVariables);
//            }
//            if (includeParameters)
//            {
//                variables = variables.Concat(Parameters);
//            }
//            return Expression.Block(variables, body);
//        }

//        public virtual bool IsRoot => false;

//        public string? Name { get; }

//        //public ExpressionExtensionData<TData> AddAndCreateLocalVariable_Generic<TData>(string name, out BinaryExpression assignExpression,
//        //    Type[] typeArguments,
//        //    params Expression[] constructorArguments)
//        //{
//        //    var variable = Expression.Variable(typeof(TData), name);
//        //    _LocalVariables.Add(name, variable);
//        //    if (ExpressionExtensionData.TryCreate_Generic<TData>(variable, out var expressionExtensionData,
//        //        out var newExpression, typeArguments, constructorArguments) == false || expressionExtensionData == null)
//        //    {
//        //        throw new NotImplementedException(); // Couldn't create variable
//        //    }
//        //    assignExpression = Expression.Assign(variable, newExpression);
//        //    return expressionExtensionData;
//        //}
//        //public ExpressionExtensionData<TData> AddAndCreateLocalVariable<TData>(string name, out BinaryExpression assignExpression, 
//        //    params Expression[] constructorArguments)
//        //{
//        //    return AddAndCreateLocalVariable_Generic<TData>(name, out assignExpression, Type.EmptyTypes, constructorArguments);
//        //}

//        public virtual ParameterExpression AddParameter(string name, Expression assignValue, out BinaryExpression assignExpression)
//        {
//            return AddVariable(_LocalVariables, name, assignValue, out assignExpression, parameter: true);
//        }
//        public virtual ParameterExpression AddLocalVariable(string name, Expression assignValue, out BinaryExpression assignExpression)
//        {
//            return AddVariable(_LocalVariables, name, assignValue, out assignExpression);
//        }

//        public ParameterExpression AddInternalVariable(string name, Expression assignValue, out BinaryExpression assignExpression)
//        {
//            return AddVariable(_InternalVariables, name, assignValue, out assignExpression);
//        }

//        private ParameterExpression AddVariable(Dictionary<string, ParameterExpression> parameterDictionary,
//            string name, Expression assignValue, out BinaryExpression assignExpression, bool parameter = false)
//        {
//            var variable = Expression.Variable(assignValue.Type, name);
//            parameterDictionary.Add(name, variable);
//            assignExpression = Expression.Assign(variable, assignValue);
//            if(parameter)
//            {
//                Parameters.Add(variable);
//            }
//            return variable;
//        }

//        public ParameterExpression this[string internalVariableName]
//        {
//            get => _InternalVariables[internalVariableName];
//        }

//        public virtual bool TryGetVariable(string variableName, [NotNullWhen(true)]out ParameterExpression? expression)
//        {
//            if (_LocalVariables.TryGetValue(variableName, out expression))
//            {
//                return true;
//            }

//            return ParentScope != null ? 
//                ParentScope.TryGetVariable(variableName, out expression) : 
//                false;
//        }

//        public virtual Scope FindRootScope()
//        {
//            return ParentScope?.FindRootScope() ?? this;
//        }

//        public Scope CreateChild(string? name = null)
//        {
//            return new Scope(this, name);
//        }

//        public IDictionary<string, Expression> VariableWalk()
//        {
//            return VariableWalk(new Dictionary<string, Expression>());
//        }
//        protected virtual IDictionary<string, Expression> VariableWalk(IDictionary<string, Expression> dict)
//        {
//            foreach (var key in _LocalVariables.Keys)
//            {
//                if (dict.ContainsKey(key) == false)
//                {
//                    dict.Add(key, _LocalVariables[key]);
//                }
//            }
//            return ParentScope?.VariableWalk(dict) ?? dict;
//        }


//    }
//}
