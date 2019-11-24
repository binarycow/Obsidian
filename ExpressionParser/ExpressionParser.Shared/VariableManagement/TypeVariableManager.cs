//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace ExpressionParser.VariableManagement
//{
//    public class TypeVariableManager : IVariableManager<Type>
//    {
//        public TypeVariableManager(IDictionary<string, object?> variables)
//        {
//            foreach(var key in variables.Keys)
//            {
//                AddVariable(key, variables[key]?.GetType() ?? typeof(object));
//            }
//        }

//        private readonly Dictionary<string, VariableInfo> _ByName = new Dictionary<string, VariableInfo>();
//        private readonly Dictionary<int, VariableInfo> _ByIndex = new Dictionary<int, VariableInfo>();
//        private int _NextIndex = 0;

//        public VariableInfo AddVariable(string name, Type value)
//        {
//            if(_ByName.ContainsKey(name))
//            {
//                return _ByName[name];
//            }
//            var variableInfo = new VariableInfo(name, value, _NextIndex++);
//            _ByName.Add(name, variableInfo);
//            _ByIndex.Add(variableInfo.Index, variableInfo);
//            return variableInfo;
//        }

//        public bool TryGetVariable(string name, out VariableInfo variableInfo)
//        {
//            return _ByName.TryGetValue(name, out variableInfo);
//        }

//        internal VariableInfo<Type>[] ToArray()
//        {
//            return _ByIndex.Keys.OrderBy(key => key).Select(key => _ByIndex[key]).ToArray();
//        }
//    }
//}
